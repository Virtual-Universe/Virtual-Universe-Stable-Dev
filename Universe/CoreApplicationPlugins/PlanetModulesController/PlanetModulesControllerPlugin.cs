/*
 * Copyright (c) Contributors, http://virtual-planets.org/
 * See CONTRIBUTORS.TXT for a full list of copyright holders.
 * For an explanation of the license of each contributor and the content it 
 * covers please see the Licenses directory.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of the Virtual-Universe Project nor the
 *       names of its contributors may be used to endorse or promote products
 *       derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE DEVELOPERS ``AS IS'' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE CONTRIBUTORS BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using Nini.Config;
using Universe.Framework.ConsoleFramework;
using Universe.Framework.ModuleLoader;
using Universe.Framework.Modules;
using Universe.Framework.SceneInfo;
using Universe.Framework.Services;

namespace Universe.CoreApplicationPlugins.PlanetModulesController
{
    public class PlanetModulesControllerPlugin : IPlanetModulesController, IApplicationPlugin
    {
        protected List<IPlanetModuleBase> IPlanetModuleBaseModules = new List<IPlanetModuleBase>();

        // Config access

        // Our name
        const string m_name = "PlanetModulesControllerPlugin";
        ISimulationBase m_simBase;

        #region IApplicationPlugin Members

        public string Name
        {
            get { return m_name; }
        }

        #endregion

        #region IPlanetModulesController implementation

        // The root of all evil.
        // This is where we handle adding the modules to scenes when they
        // load. This means that here we deal with replaceable interfaces,
        // non-shared modules, etc.
        protected Dictionary<IScene, Dictionary<string, IPlanetModuleBase>> PlanetModules =
            new Dictionary<IScene, Dictionary<string, IPlanetModuleBase>>();

        public void AddPlanetToModules(IScene scene)
        {
            Dictionary<Type, INonSharedPlanetModule> deferredNonSharedModules =
                new Dictionary<Type, INonSharedPlanetModule>();

            // We need this to see if a module has already been loaded and
            // has defined a replaceable interface. It's a generic call,
            // so this can't be used directly. It will be used later
            Type s = scene.GetType();
            MethodInfo mi = s.GetMethod("RequestModuleInterface");

            // Scan for, and load, non-shared modules
            List<INonSharedPlanetModule> list = new List<INonSharedPlanetModule>();
            List<INonSharedPlanetModule> m_nonSharedModules = UniverseModuleLoader.PickupModules<INonSharedPlanetModule>();
            foreach (INonSharedPlanetModule module in m_nonSharedModules)
            {
                Type replaceableInterface = module.ReplaceableInterface;
                if (replaceableInterface != null)
                {
                    MethodInfo mii = mi.MakeGenericMethod(replaceableInterface);

                    if (mii.Invoke(scene, new object[0]) != null)
                    {
                        MainConsole.Instance.DebugFormat("[Planet Module]: Not loading {0} because another module has registered {1}",
                                          module.Name, replaceableInterface);
                        continue;
                    }

                    deferredNonSharedModules[replaceableInterface] = module;
                    MainConsole.Instance.DebugFormat("[Planet Module]: Deferred load of {0}", module.Name);
                    continue;
                }

                //MainConsole.Instance.DebugFormat("[Planet Module]: Adding scene {0} to non-shared module {1}",
                //                  scene.PlanetInfo.PlanetName, module.Name);

                // Initialize the module
                module.Initialize(m_simBase.ConfigSource);

                IPlanetModuleBaseModules.Add(module);
                list.Add(module);
            }

            // Now add the modules that we found to the scene. If a module
            // wishes to override a replaceable interface, it needs to
            // register it in Initialize, so that the deferred module
            // won't load.
            foreach (INonSharedPlanetModule module in list)
            {
                try
                {
                    module.AddPlanet(scene);
                }
                catch (Exception ex)
                {
                    MainConsole.Instance.ErrorFormat("[Planet Modules Controller Plugin]: Failed to load module {0}: {1}", module.Name, ex.ToString());
                }

                AddPlanetModule(scene, module.Name, module);
            }

            // Same thing for non-shared modules, load them unless overridden
            List<INonSharedPlanetModule> deferredlist = new List<INonSharedPlanetModule>();

            foreach (INonSharedPlanetModule module in deferredNonSharedModules.Values)
            {
                // Check interface override
                Type replaceableInterface = module.ReplaceableInterface;
                if (replaceableInterface != null)
                {
                    MethodInfo mii = mi.MakeGenericMethod(replaceableInterface);

                    if (mii.Invoke(scene, new object[0]) != null)
                    {
                        MainConsole.Instance.DebugFormat("[Planet Module]: Not loading {0} because another module has registered {1}",
                                          module.Name, replaceableInterface);
                        continue;
                    }
                }

                MainConsole.Instance.DebugFormat("[Planet Module]: Adding scene {0} to non-shared module {1} (deferred)",
                                  scene.PlanetInfo.PlanetName, module.Name);

                try
                {
                    module.Initialize(m_simBase.ConfigSource);
                }
                catch (Exception ex)
                {
                    MainConsole.Instance.ErrorFormat("[Planet Modules Controller Plugin]: Failed to load module {0}: {1}", module.Name, ex.ToString());
                }

                IPlanetModuleBaseModules.Add(module);
                list.Add(module);
                deferredlist.Add(module);
            }

            // Finally, load valid deferred modules
            foreach (INonSharedPlanetModule module in deferredlist)
            {
                try
                {
                    module.AddPlanet(scene);
                }
                catch (Exception ex)
                {
                    MainConsole.Instance.ErrorFormat("[Planet Modules Controller Plugin]: Failed to load module {0}: {1}", module.Name, ex.ToString());
                }

                AddPlanetModule(scene, module.Name, module);
            }

            // This is needed for all module types. Modules will register
            // Interfaces with scene in AddScene, and will also need a means
            // to access interfaces registered by other modules. Without
            // this extra method, a module attempting to use another modules's
            // interface would be successful only depending on load order,
            // which can't be depended upon, or modules would need to resort
            // to ugly things in to attempt to request interfaces when needed
            // and unnecessary caching logic repeated in all modules.
            // The extra function stub is just that much cleaner
            foreach (INonSharedPlanetModule module in list)
            {
                try
                {
                    module.PlanetLoaded(scene);
                }
                catch (Exception ex)
                {
                    MainConsole.Instance.ErrorFormat("[Planet Modules Controller Plugin]: Failed to load module {0}: {1}", module.Name, ex.ToString());
                }
            }
        }

        public void RemovePlanetFromModules(IScene scene)
        {
            foreach (IPlanetModuleBase module in PlanetModules[scene].Values)
            {
                MainConsole.Instance.DebugFormat("[Planet Module]: Removing scene {0} from module {1}",
                                  scene.PlanetInfo.PlanetName, module.Name);
                module.RemovePlanet(scene);
                if (module is INonSharedPlanetModule)
                {
                    // as we were the only user, this instance has to die
                    module.Close();
                }
            }

            PlanetModules[scene].Clear();
        }

        void AddPlanetModule(IScene scene, string p, IPlanetModuleBase module)
        {
            if (!PlanetModules.ContainsKey(scene))
                PlanetModules.Add(scene, new Dictionary<string, IPlanetModuleBase>());
            PlanetModules[scene][p] = module;
        }

        #endregion

        #region IPlanetModulesController Members

        public List<IPlanetModuleBase> AllModules
        {
            get { return IPlanetModuleBaseModules; }
        }

        #endregion

        #region IApplicationPlugin implementation

        public void PreStartup(ISimulationBase simBase)
        {
        }

        public void Initialize(ISimulationBase simBase)
        {
            m_simBase = simBase;

            IConfig handlerConfig = simBase.ConfigSource.Configs["ApplicationPlugins"];
            if (handlerConfig.GetString("PlanetModulesControllerPlugin", "") != Name)
                return;

            m_simBase.ApplicationRegistry.RegisterModuleInterface<IPlanetModulesController>(this);
        }

        public void ReloadConfiguration(IConfigSource config)
        {
            //Update all modules that we have here
            foreach (IPlanetModuleBase module in AllModules)
            {
                try
                {
                    module.Initialize(config);
                }
                catch (Exception ex)
                {
                    MainConsole.Instance.ErrorFormat("[Planet Modules Controller Plugin]: Failed to load module {0}: {1}", module.Name, ex.ToString());
                }
            }
        }

        public void PostInitialize()
        {
        }

        public void Start()
        {
        }

        public void PostStart()
        {
        }

        public void Close()
        {
        }

        #endregion
    }
}
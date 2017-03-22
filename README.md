# Attention all users
We are aware of an issue involving the latest versions of Third Party Viewers (TPVs) that causes avatars to go black when attaching or detaching items. The Second Galaxy Development Team is aware of this bug and is currently investigating it.

Viewers affected are: Singularity Viewer (1.8.7.6861), Firestorm (4.7.9), Alchemy Viewer (4.0.1)  Kokua Viewer is not yet known to be affected.

Until we get this fixed we advise all users to use the version of their viewer just before the current release version.  In this way you will avoid this bug while we investigate this issue further.

# Virtual-Dev
The development repository for the Virtual Universe Simulator

- Development Version: 1.0.3
- Version Code Name: Earth
- Version Release Schedule
  - Release Candidate 1: February 13, 2017
  - Release Candidate 2: April 17, 2017
  - Release: June 19, 2017


This repository is the development repository and should not be considered in any means stable.  If you are using our architecture as the base for a production level grid please use the Virtual-Universe/Virtual-Universe repository.

If you would like to help by contributing code please see our How to contribute document.

Thank you

The Virtual Universe Development Team

The Virtual Universe project is owned by the Second Galaxy Development Team.  A group dedicated to the development of virtual world grid architecture that promotes the future of virtual worlds.


*NOTES:*

*- As of March 22, 2017, the LibOMV libraries are included as a submodule of the Virtual Universe repositories. When cloning, ensure that the submodules are included.*

`git clone --recursive https://github.com/Virtual-Universe/Virtual-LibOMV.git`

To update an existing repository that does not have the LibOMV submodule

	cd <your Virtual Universe repository>
	git submodule init
	git submodule update

*If you do not know what submodules are, or you are not using git from the command line, PLEASE make sure to fetch the submodules too.*

**If you download the repo using the zip file option, you will also need to download the Virtual-LibOMV submodule and extract it in your local Virtual Universe repo.**
`https://github.com/Virtual-Universe/Virtual-LibOMV`
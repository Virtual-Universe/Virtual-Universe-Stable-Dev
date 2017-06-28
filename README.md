# Attention all users
If you are using Virtual Universe to run a production level grid open to the public, please use the official stable code in the Virtual-Universe Repository.

The repository is at: https://github.com/Virtual-Universe/Virtual-Universe

Never use the code in the Virtual-Dev repository for your production level grids.  The code in the Development Repository is for development of Virtual Universe and is not considered stable.

# Virtual-Dev
The development repository for the Virtual Universe Simulator

- Development Version: 1.0.4
- Version Code Name: Vulcan
- Version Release Schedule
  - Release Candidate 1: February 11, 2018
  - Release Candidate 2: April 15, 2018
  - Release: June 17, 2018


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
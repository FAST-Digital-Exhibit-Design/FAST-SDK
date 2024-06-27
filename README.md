# FAST SDK

A software development kit for creating FAST digital exhibit experiences
in Unity.

**What is FAST?**

<ins>F</ins>lexible, <ins>A</ins>ccessible <ins>S</ins>trategies for <ins>T</ins>imely Digital Exhibit Design

Museums play a critical role in engaging their communities around urgent 
issues that emerge in the public sphere. However, typical exhibit development 
timelines can stretch for years, and "what's new" can change swiftly before 
an exhibition is even launched. What if museums could stay agile and dynamic, 
changing out content as community needs shift, science progresses, and the 
world changes? What if timely exhibit offerings could be not just efficiently 
produced, but accessible and welcoming to all visitors? This vision of 
efficiency and accessibility is at the center of FAST.

For more information about FAST, please see:

* FAST Booklet, an introduction and explination of the FAST project
* FAST Toolkit, a repository off all assets and documentation published from the FAST project

## System Requirements

### Runtime Requirements

* Microsoft Windows 10 or 11

### Build Requirements

* Unity Editor 2022.3.0 or later
* Unity Api Compatibility Level set to .NET Framework
* [NaughtyAttributes](https://github.com/dbrizov/NaughtyAttributes) package

### Installation Requirements (if installing using a Git URL)

* Install the [Git client](https://git-scm.com/) (minimum version 2.14.0) 
on your computer.
* On Windows, add the Git executable path to the `PATH` system environment 
variable.

## Installation

1. Verify Unity's Api Compatibility Level by navigating to Unity's main 
menu and selecting **Edit > Project Settings > Player > Other settings**. 
Under the Configuration heading, set **Api Compatibility Level** to 
<b>.NET Framework</b>.

2. Open the Package Manager window by navigating to Unity's main menu 
and selecting **Window > Package Manager**.

3. Install the NaughtyAttributes package using **Add package from git URL** 
from the Unity Package Manager:

    ```
    https://github.com/dbrizov/NaughtyAttributes.git#upm
    ```

4. Install the FAST SDK package using **Add package from git URL** 
from the Unity Package Manager:

    ```
    https://github.com/FAST-Digital-Exhibit-Design/FAST-SDK.git
    ```

    * Or, download the source code `.zip` or `.tar.gz` file from 
    [Releases](https://github.com/FAST-Digital-Exhibit-Design/FAST-SDK/releases) 
    and extract it locally. Then install the FAST SDK package using 
    **Add package from disk** from the Unity Package Manager.

For more infomation about installing Unity packages, see:

* [Install a UPM package from a Git URL](https://docs.unity3d.com/2022.3/Documentation/Manual/upm-ui-giturl.html)
* [Install a UPM package from a local folder](https://docs.unity3d.com/2022.3/Documentation/Manual/upm-ui-local.html)

## Documentation

See the [Reference Manual](https://FAST-Digital-Exhibit-Design.github.io/FAST-SDK-Documentation/)

## Contributions

This repo is only maintained with bug fixes and Pull Requests are not accepted 
at this time. If you'd like to contribute, please post questions and 
comments about using FAST SDK to [Discussions](https://github.com/FAST-Digital-Exhibit-Design/FAST-SDK/discussions) 
and report bugs using [Issues](https://github.com/FAST-Digital-Exhibit-Design/FAST-SDK/issues).

## Citation

If you reference this software in a publication, please cite it as follows:

**APA**
```
Museum of Science, Boston. FAST SDK (Version 1.0.0) [Computer software]. https://github.com/FAST-Digital-Exhibit-Design/FAST-SDK
```

**BibTeX**
```
@software{Museum_of_Science_FAST_SDK,
author = {{Museum of Science, Boston}},
license = {MIT},
title = {{FAST SDK}},
url = {https://github.com/FAST-Digital-Exhibit-Design/FAST-SDK},
version = {1.0.0}
}
```

## Notices

Copyright (C) 2024 Museum of Science, Boston
<https://www.mos.org/>

This software was developed through a grant to the Museum of Science, 
Boston from the Institute of Museum and Library Services under Award 
#MG-249646-OMS-21. For more information about this grant, see 
<https://www.imls.gov/grants/awarded/mg-249646-oms-21>.

This software is open source: you can redistribute it and/or modify
it under the terms of the MIT License.

This software is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
MIT License for more details.

You should have received a copy of the MIT License along with this 
software. If not, see <https://opensource.org/license/MIT>.

`SPDX-License-Identifier: MIT`

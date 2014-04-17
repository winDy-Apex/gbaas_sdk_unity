#!/bin/sh
cp /Users/winDy/Documents/windystudio_svn/PROJECT/Apexplatform/gbaas_sdk_unity/GBaaS.io/bin/Debug/* /Users/winDy/Documents/windystudio_svn/PROJECT/Apexplatform/Test\ Unity\ Project/Assets/Plugins/
cp /Users/winDy/Documents/windystudio_svn/PROJECT/Apexplatform/gbaas_sdk_unity/GBaaS.io/bin/Debug/* /Users/winDy/Documents/windystudio_svn/PROJECT/Apexplatform/Test\ Photon\ Project/Assets/Plugins/
rm -rf /Users/winDy/Documents/windystudio_svn/PROJECT/Apexplatform/unity2d_testgame/Assets/Standard\ Assets/GBaaS/
mkdir /Users/winDy/Documents/windystudio_svn/PROJECT/Apexplatform/unity2d_testgame/Assets/Standard\ Assets/GBaaS/
cp /Users/winDy/Documents/windystudio_svn/PROJECT/Apexplatform/gbaas_sdk_unity/GBaaS.io/bin/Debug/* /Users/winDy/Documents/windystudio_svn/PROJECT/Apexplatform/unity2d_testgame/Assets/Standard\ Assets/GBaaS/
cp /Users/winDy/Documents/windystudio_svn/PROJECT/Apexplatform/gbaas_sdk_unity/GBaaS.io/GBaaSApi.cs /Users/winDy/Documents/windystudio_svn/PROJECT/Apexplatform/unity2d_testgame/Assets/Standard\ Assets/GBaaS/
cp /Users/winDy/Documents/windystudio_svn/PROJECT/Apexplatform/gbaas_sdk_unity/GBaaS.io/GBaaSApiHandler.cs /Users/winDy/Documents/windystudio_svn/PROJECT/Apexplatform/unity2d_testgame/Assets/Standard\ Assets/GBaaS/
cp -r /Users/winDy/Documents/windystudio_svn/PROJECT/Apexplatform/gbaas_sdk_unity/GBaaS.io/Objects /Users/winDy/Documents/windystudio_svn/PROJECT/Apexplatform/unity2d_testgame/Assets/Standard\ Assets/GBaaS/
cp -r /Users/winDy/Documents/windystudio_svn/PROJECT/Apexplatform/gbaas_sdk_unity/GBaaS.io/Services /Users/winDy/Documents/windystudio_svn/PROJECT/Apexplatform/unity2d_testgame/Assets/Standard\ Assets/GBaaS/
cp -r /Users/winDy/Documents/windystudio_svn/PROJECT/Apexplatform/gbaas_sdk_unity/GBaaS.io/Utils /Users/winDy/Documents/windystudio_svn/PROJECT/Apexplatform/unity2d_testgame/Assets/Standard\ Assets/GBaaS/
rm /Users/winDy/Documents/windystudio_svn/PROJECT/Apexplatform/unity2d_testgame/Assets/Standard\ Assets/GBaaS/GBaaS.io.dll
rm /Users/winDy/Documents/windystudio_svn/PROJECT/Apexplatform/unity2d_testgame/Assets/Standard\ Assets/GBaaS/GBaaS.io.dll.mdb
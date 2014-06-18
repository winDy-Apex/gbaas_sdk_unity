#!/bin/sh
deploy_source="/Users/winDy/Documents/windystudio_svn/PROJECT/Apexplatform/gbaas_sdk_unity/GBaaS.io/bin/Release/*"
deploy_gcmjar="/Users/winDy/Documents/windystudio_svn/PROJECT/Apexplatform/unity-gcm/source/UnityGcmPlugin/bin/unitygcmplugin.jar"

deploy_target1="/Users/winDy/Documents/windystudio_svn/PROJECT/Apexplatform/Test Unity Project/Assets/Plugins/"
deploy_target2="/Users/winDy/Documents/windystudio_svn/PROJECT/Apexplatform/Test Photon Project/Assets/Plugins/"
deploy_target3="/Users/winDy/Documents/windystudio_svn/PROJECT/Apexplatform/unity2d_testgame/Assets/Standard Assets/GBaaS/"

deploy_target_gcmjar="/Users/winDy/Documents/windystudio_svn/PROJECT/Apexplatform/unity2d_testgame/Assets/Plugins/Android/"

cp $deploy_source "$deploy_target1"
cp $deploy_source "$deploy_target2"

rm -rf "$deploy_target3"
mkdir "$deploy_target3"

#dll 복사
cp $deploy_source "$deploy_target3"

#gcmjar 복사
cp $deploy_gcmjar "$deploy_target_gcmjar"

#소스 복사
#cp /Users/winDy/Documents/windystudio_svn/PROJECT/Apexplatform/gbaas_sdk_unity/GBaaS.io/GBaaSApi.cs /Users/winDy/Documents/windystudio_svn/PROJECT/Apexplatform/unity2d_testgame/Assets/Standard\ Assets/GBaaS/
#cp /Users/winDy/Documents/windystudio_svn/PROJECT/Apexplatform/gbaas_sdk_unity/GBaaS.io/GBaaSApiHandler.cs /Users/winDy/Documents/windystudio_svn/PROJECT/Apexplatform/unity2d_testgame/Assets/Standard\ Assets/GBaaS/
#cp -r /Users/winDy/Documents/windystudio_svn/PROJECT/Apexplatform/gbaas_sdk_unity/GBaaS.io/Objects /Users/winDy/Documents/windystudio_svn/PROJECT/Apexplatform/unity2d_testgame/Assets/Standard\ Assets/GBaaS/
#cp -r /Users/winDy/Documents/windystudio_svn/PROJECT/Apexplatform/gbaas_sdk_unity/GBaaS.io/Services /Users/winDy/Documents/windystudio_svn/PROJECT/Apexplatform/unity2d_testgame/Assets/Standard\ Assets/GBaaS/
#cp -r /Users/winDy/Documents/windystudio_svn/PROJECT/Apexplatform/gbaas_sdk_unity/GBaaS.io/Utils /Users/winDy/Documents/windystudio_svn/PROJECT/Apexplatform/unity2d_testgame/Assets/Standard\ Assets/GBaaS/
#rm /Users/winDy/Documents/windystudio_svn/PROJECT/Apexplatform/unity2d_testgame/Assets/Standard\ Assets/GBaaS/GBaaS.io.dll
#rm /Users/winDy/Documents/windystudio_svn/PROJECT/Apexplatform/unity2d_testgame/Assets/Standard\ Assets/GBaaS/GBaaS.io.dll.mdb

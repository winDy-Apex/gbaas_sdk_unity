# -*- coding: cp949 -*-
#!/usr/bin/python
import sys
import os


def runDeploy(source_root, deployMode):
	#deploy_source
	_deploy_source = source_root + "/gbaas_sdk_unity/GBaaS.io/bin/" + deployMode + "/*"
	_deploy_gcmjar = source_root + "/unity-gcm/source/UnityGcmPlugin/bin/unitygcmplugin.jar"
	_deploy_target1 = source_root + "/Test\ Unity\ Project/Assets/Plugins/"
	_deploy_target2 = source_root + "/Test\ Photon\ Project/Assets/Plugins/"
	_deploy_target3 = source_root + "/unity2d_testgame/Assets/Standard\ Assets/GBaaS/"
	_deploy_target_gcmjar = source_root + "/unity2d_testgame/Assets/Plugins/Android/"
	
	print os.popen("cp " + _deploy_source + " " + _deploy_target1).read();
	print os.popen("cp " + _deploy_source + " " + _deploy_target2).read();
	print os.popen("rm -rf " + _deploy_target3).read();
	print os.popen("mkdir " + _deploy_target3).read(); 
	#dll 복사
	print os.popen("cp " + _deploy_source + " " + _deploy_target3).read();
	#gcmjar 복사
	print os.popen("cp " + _deploy_gcmjar + " " + _deploy_target_gcmjar).read();


def main( argv = None ):
	username = os.popen("id -u -n").read().strip()
	source_root = ""
	deployMode = "Debug" #deployMode must be 'Debug' or 'Release'
	
	if len(sys.argv) >= 2:
		deployMode = sys.argv[1]
	
	print "Deploy Set for : " + username
	print "Deploy Mode is : " + deployMode
	
	if username == "winDy":
		print "Run Deploy script for winDy"
		source_root = "/Users/winDy/Documents/windystudio_svn/PROJECT/Apexplatform"
		runDeploy(source_root, deployMode)
	else:
		print "You are not preset User. So we Skip Deploy script. Make Your Own Code in deploydll.py or Please Run Manually."

if __name__ == "__main__":
     sys.exit( main() )
     
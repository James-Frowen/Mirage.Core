if [ -z "$1" ]; then
  echo "Usage: bash ./CopyFromMirage.sh /path/to/Mirage/"
  echo "Ex: bash ./CopyFromMirage.sh ../Mirage/"
  exit 1
else
  MiragePath=$1
  echo "Copying Mirage Updates to Mirage.Core"
  echo "Mirage Path:  $MiragePath"
  
fi 

CopyScripts () {
	# make the directory if it doesn't exist already...
	if [ ! -d "$2" ]; then
		echo "Creating directory: $2"
		mkdir -p "$2"
	#else
	#	rm $2/*.cs
	fi
    
    # add -v to cp for verbose logging
    find $1/*.cs | xargs cp -vt $2
}

# #echo
# #echo "Applying Mirage.Core changes..."
# # Directories not currently included in Runtime Copy. Need to patch them out of unity?
# #CopyScripts "$MiragePath/Assets/Mirage/Runtime" "./src/Mirage.Core"
# #CopyScripts "$MiragePath/Assets/Mirage/Runtime/Authentication" "./src/Mirage.Core/Authentication"
# #CopyScripts "$MiragePath/Assets/Mirage/Runtime/Collections" "./src/Mirage.Core/Collections"
# #CopyScripts "$MiragePath/Assets/Mirage/Runtime/Extensions" "./src/Mirage.Core/Extensions"
# #CopyScripts "$MiragePath/Assets/Mirage/Runtime/RemoteCalls" "./src/Mirage.Core/RemoteCalls"

echo
echo "Applying Mirage.Core/Events changes..."
CopyScripts "$MiragePath/Assets/Mirage/Runtime/Events" "./src/Mirage.Core/Events"
rm "./src/Mirage.Core/Events/AddLateEvent_new.cs"
rm "./src/Mirage.Core/Events/BoolAddLateEvent.cs"
rm "./src/Mirage.Core/Events/DisconnectAddLateEvent.cs"
rm "./src/Mirage.Core/Events/NetworkPlayerAddLateEvent.cs"

echo
echo "Applying Mirage.Core/Serialization changes..."
CopyScripts "$MiragePath/Assets/Mirage/Runtime/Serialization" "./src/Mirage.Core/Serialization"
rm "./src/Mirage.Core/Serialization/CompressedExtensions.cs"
rm "./src/Mirage.Core/Serialization/MirageTypesExtensions.cs"
rm "./src/Mirage.Core/Serialization/UnityTypesExtensions.cs"

echo
echo "Applying Mirage.Core/Sockets changes..."
CopyScripts "$MiragePath/Assets/Mirage/Runtime/Sockets" "./src/Mirage.Core/Sockets"
CopyScripts "$MiragePath/Assets/Mirage/Runtime/Sockets/Udp" "./src/Mirage.Core/Sockets/Udp"
rm "./src/Mirage.Core/Sockets/Udp/InitUDP.cs"
rm "./src/Mirage.Core/Sockets/Udp/NanoEndPoint.cs"
rm "./src/Mirage.Core/Sockets/Udp/NanoSocket_NotSupported.cs"
rm "./src/Mirage.Core/Sockets/Udp/NanoSocket.cs"
rm "./src/Mirage.Core/Sockets/Udp/NanoSocketException.cs"

echo
echo "Applying Mirage.Core/Utils changes..."
CopyScripts "$MiragePath/Assets/Mirage/Runtime/Utils" "./src/Mirage.Core/Utils"

echo
echo "Applying Mirage.Logging changes..."
CopyScripts "$MiragePath/Assets/Mirage/Runtime/Logging" "./src/Mirage.Logging"
rm "./src/Mirage.Logging/EditorLogSettingsLoader.cs"
rm "./src/Mirage.Logging/LogSettings.cs"
rm "./src/Mirage.Logging/MirageLogHandler.cs"

echo
echo "Applying Mirage.SocketLayer changes..."
CopyScripts "$MiragePath/Assets/Mirage/Runtime/SocketLayer" "./src/Mirage.SocketLayer"
CopyScripts "$MiragePath/Assets/Mirage/Runtime/SocketLayer/Connection" "./src/Mirage.SocketLayer/Connection"
CopyScripts "$MiragePath/Assets/Mirage/Runtime/SocketLayer/ConnectionTrackers" "./src/Mirage.SocketLayer/ConnectionTrackers"
CopyScripts "$MiragePath/Assets/Mirage/Runtime/SocketLayer/Enums" "./src/Mirage.SocketLayer/Enums"

echo
echo "Applying Mirage.CodeGen/Weaver changes..."
CopyScripts "$MiragePath/Assets/Mirage/Weaver" "./src/Mirage.CodeGen/Weaver"
CopyScripts "$MiragePath/Assets/Mirage/Weaver/Processors" "./src/Mirage.CodeGen/Weaver/Processors"
CopyScripts "$MiragePath/Assets/Mirage/Weaver/Serialization" "./src/Mirage.CodeGen/Weaver/Serialization"
CopyScripts "$MiragePath/Assets/Mirage/Weaver/Mirage.CecilExtensions" "./src/Mirage.CodeGen/Mirage.CecilExtensions"
CopyScripts "$MiragePath/Assets/Mirage/Weaver/Mirage.CecilExtensions/Extensions" "./src/Mirage.CodeGen/Mirage.CecilExtensions/Extensions"
CopyScripts "$MiragePath/Assets/Mirage/Weaver/Mirage.CecilExtensions/Logging" "./src/Mirage.CodeGen/Mirage.CecilExtensions/Logging"
CopyScripts "$MiragePath/Assets/Mirage/Weaver/Mirage.CecilExtensions/UnityCodeGen" "./src/Mirage.CodeGen/Mirage.CecilExtensions/UnityCodeGen"
rm "./src/Mirage.CodeGen/Weaver/MirageILPostProcessor.cs"
rm "./src/Mirage.CodeGen/Weaver/Processors/AttributeProcessor.cs"
rm "./src/Mirage.CodeGen/Mirage.CecilExtensions/AssemblyInfo.cs"
rm "./src/Mirage.CodeGen/Weaver/Processors/ClientRpcProcessor.cs"
rm "./src/Mirage.CodeGen/Weaver/Processors/NetworkBehaviourProcessor.cs"
rm "./src/Mirage.CodeGen/Weaver/Processors/RegisterRpc.cs"
rm "./src/Mirage.CodeGen/Weaver/Processors/RpcMethod.cs"
rm "./src/Mirage.CodeGen/Weaver/Processors/RpcProcessor.cs"
rm "./src/Mirage.CodeGen/Weaver/Processors/ServerRpcProcessor.cs"
rm "./src/Mirage.CodeGen/Weaver/Processors/SyncObjectProcessor.cs"
rm "./src/Mirage.CodeGen/Weaver/Processors/SyncVarProcessor.cs"
rm "./src/Mirage.CodeGen/Weaver/Serialization/QuaternionFinder.cs"
rm "./src/Mirage.CodeGen/Weaver/Serialization/Vector2Finder.cs"
rm "./src/Mirage.CodeGen/Weaver/Serialization/Vector3Finder.cs"

echo
echo "Applying Mirage.Test/Common changes..."
CopyScripts "$MiragePath/Assets/Tests/Common" "./src/Mirage.Tests/Common"
## CopyScripts "$MiragePath/Assets/Tests/Common/Setup" "./src/Mirage.Tests/Common/Setup"
## CopyScripts "$MiragePath/Assets/Tests/Common/Setup/Instances" "./src/Mirage.Tests/Common/Setup/Instances"
rm "./src/Mirage.Tests/Common/AsyncUtil.cs"
rm "./src/Mirage.Tests/Common/BuildReleaseTestPlayer.cs"
rm "./src/Mirage.Tests/Common/EmptyBehaviour.cs"
rm "./src/Mirage.Tests/Common/LocalConnections.cs"
rm "./src/Mirage.Tests/Common/LogErrorChecker.cs"
rm "./src/Mirage.Tests/Common/MockRpcComponent.cs"
rm "./src/Mirage.Tests/Common/MockVisibility.cs"
rm "./src/Mirage.Tests/Common/PlayerWithTestsCallbacks.cs"
rm "./src/Mirage.Tests/Common/SyncObjectHelper.cs"
rm "./src/Mirage.Tests/Common/TestExtensions.cs"
rm "./src/Mirage.Tests/Common/UnityEventUtis.cs"

## Any of this relevant for Mirage > Mirage.Core Updates?
# sed -i 's/"Unity\.Mirage\.CodeGen"/"Mirage.CodeGen"/g' ./Mirage/Runtime/AssemblyInfo.cs
# echo "[assembly: InternalsVisibleTo(\"Mirage.Standalone\")]" >> ./Mirage/Runtime/AssemblyInfo.cs
# rm "./Mirage.CodeGen/Weaver/MirageILPostProcessor.cs"
# rm "./Mirage.CodeGen/Weaver/Mirage.CecilExtensions/UnityCodeGen/AssemblyInfo.cs"
# cp "$MiragePath/Assets/Mirage/Runtime/Logging/LogFactory.cs" "./Mirage.Logging/Logging/"
# sed -i 's/new Logger(createLoggerForType.Invoke(loggerName))/new StandaloneLogger()/g' ./Mirage.Logging/Logging/LogFactory.cs

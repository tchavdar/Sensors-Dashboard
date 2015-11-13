// This file is included in your project to ensure that
// the reference to the runtime library for your project
// is not stripped out by the build process.
using BrightstarDB;

namespace MQTT_WPF_Client
{
    public static class BrightstarDBForceReference
    {
        static BrightstarDBForceReference()
        {
            ConfigurationProvider dummy = null;
        }
    }
}
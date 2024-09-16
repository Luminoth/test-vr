namespace VrTest.Util;

// TODO: this code would be easier to deal with if this was an autoload singleton
// (because it has direct access to the SceneTree that way)
public static class OculusHelper
{
    private static Node OculusManager => (Engine.GetMainLoop() as SceneTree).Root.GetNode("OculusManager");

    public static bool IsOculusInitialized => OculusManager.GetIndexed("oculus_initialized").AsBool();
}

extends Node

var oculus_initialized = false

func _ready():
    # PC not supported by the Platform SDK plugin
    if OS.get_name() != "android":
        return

    GDOculusPlatform.initialize_android_async("12345") \
    .then(func(_initialization_resp):
        # TODO
        print("Oculus: " + _initialization_resp)
    )

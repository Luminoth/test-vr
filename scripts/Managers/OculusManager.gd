extends Node

@export var app_id = ""
@export var skip_entitlement_check = false

var oculus_initialized = false
var oculus_entitled = false

func _ready():
    # PC not supported by the Platform SDK plugin
    if OS.get_name() != "Android":
        push_warning("Skipping Oculus Platform init on unsupported platform ", OS.get_name())
        return

    if app_id.is_empty():
        push_error("Oculus Platform init missing AppID!")
        get_tree().quit()
        return

    print("Initializing Oculus Platform ...")

    GDOculusPlatform.initialize_android_async(app_id) \
    .then(func(_initialization_resp):
        print("Oculus Platform initialized!")

        # is the user entitled to this app?
        if !skip_entitlement_check || !OS.has_feature("debug"):
            GDOculusPlatform.user_get_is_viewer_entitled() \
            .then(func(_is_viewer_entitled_resp):
                print("User is entitled!")

                oculus_initialized = true

                GDOculusPlatform.user_get_logged_in_user() \
                .then(func(resp):
                    # TODO: store this stuff
                    print("LOGGED IN USER INFO:")
                    print("--------------------")

                    print("ID: ", resp.id)
                    print("OCULUS ID: ", resp.oculus_id)
                    print("DISPLAY NAME: ", resp.display_name)
                    print("IMAGE URL: ", resp.image_url)
                    print("SMALL IMAGE URL: ", resp.small_image_url)
                ).error(func(logged_in_user_error):
                    print("Oculus Platform logged in user error: ", logged_in_user_error)
                    get_tree().quit()
                )
            ) \
            .error(func(is_viewer_entitled_err):
                push_error("User not entitled / error! ", is_viewer_entitled_err)
                get_tree().quit()
            )
        else:
            push_warning("Skipping entitlement check ...")

            oculus_initialized = true

            # TODO: have to use a fake user or something here
            # the Oculus call won't return anything
    ) \
    .error(func(initialization_err):
        push_error("Oculus Platform initialization error: ", initialization_err)
        get_tree().quit()
    )

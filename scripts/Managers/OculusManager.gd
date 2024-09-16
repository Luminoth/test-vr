extends Node

var oculus_initialized = false
@export var app_id = ""

func _ready():
    # PC not supported by the Platform SDK plugin
    if OS.get_name() != "android":
        return

    GDOculusPlatform.initialize_android_async(app_id)\
    .then(func(_initialization_resp):
        print("Oculus Platform initialized!")

        # Is the user entitled to this app?
        GDOculusPlatform.user_get_is_viewer_entitled()\
        .then(func(_is_viewer_entitled_resp):
            print("User is entitled!")
        )\
        .error(func(is_viewer_entitled_err):
            print("User not entitled / error! ", is_viewer_entitled_err)
        )

        GDOculusPlatform.user_get_logged_in_user()\
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
        )
    )\
    .error(func(initialization_err):
        print("Oculus Platform initialization error: ", initialization_err)
    )

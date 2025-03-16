workspace "Name" "Description" {

    !identifiers hierarchical

    model {
        u = person "User"
        ss = softwareSystem "pokusHttp" {
        ui = container "app UI" {
                tags "Web Application"
                mp = component "main page"{
                    description "The page that allows user to upload files."
                }

                gf = component "get files"{
                    description "The page that allows user to get files."
                }
            }
            fb = container "file database" {
                tags "Database"
                dt = component "database"{
                    description "Stores all files and informations."
                }
            }

            bo = container "BackgroundObserver" {
                 pt = component "ProcessFiles"{
                    description "requests files and theirs informations and processes them."
                }
            }
            ap = container "File Controller api" {
                gf = component "get files"{
                    description "gets files and theirs informations."
                }
                pf = component "post files"{
                    description "post files and theirs informations."
                }
            }
        }

        u -> ss.ui.mp "uploads file and its informations"
        ss.ui.mp -> ss.ap "sends file and its informations"
        ss.ap.pf -> ss.fb.dt "saves file and its informations"
        ss.bo.pt -> ss.fb.dt "requests file and its informations"
        ss.fb.dt -> ss.bo.pt "give file and its informations"
        ss.bo.pt -> ss.fb.dt "saves updated file with its informations"
        u -> ss.ui.gf "requests file and its informations"
        ss.ui.gf -> ss.ap.gf "requests for file and its informations"
        ss.ap.gf -> ss.fb.dt "requests first file in the queue and its informations"
        ss.fb.dt -> ss.ap.gf "sends file and its informations"
        ss.ap -> ss.ui.gf "gets file and its informations"
        ss.ui.gf -> u "shows file and its informations"
        
    }

    views {

        systemContext ss "c1" {
            include *
            autolayout lr
        }

        container ss "c2" {
            include *
            autolayout lr
        }
        component  ss.ui "c3_ui" {
            include *
            autolayout lr
        }
        dynamic ss "uploading_and_getting_files" {
            description "The sequence of interactions for user to upload and retrieve file."

            u -> ss.ui "uploads file and its informations"
            ss.ui -> ss.ap "sends file and its informations"
            ss.ap -> ss.fb "saves file and its informations"
            ss.bo -> ss.fb "requests file and its informations"
            ss.fb -> ss.bo "give file and its informations"
            ss.bo -> ss.fb "saves updated file with its informations"
            u -> ss.ui "requests updated file and its informations"
            ss.ui -> ss.ap "requests for the updated file and its informations"
            ss.ap -> ss.fb "requests first file in queue and its informations"
            ss.fb -> ss.ap "sends file and its informations"
            ss.ap -> ss.ui "gets file and its informations"
            ss.ui -> u "shows file and its informations"

            autolayout lr
        }
        component  ss.fb "c3_fb" {
            include *
            autolayout lr
        }
        component  ss.bo "c3_bo" {
            include *
            autolayout lr
        }
        component  ss.ap "c3_ap" {
            include *
            autolayout lr
        }

        

        styles {
            element "Element" {
                color #ffffff
            }
            element "Person" {
                background #3244CA
                shape person
            }
            element "Software System" {
                background #6472D8
            }
            element "Container" {
                background #6472D8
            }
            element "Component" {
                background #6472D8
            }
            element "Database" {
                shape cylinder
                background #3E4CB6
            }
        }
    }

    configuration {
        scope softwaresystem
    }

}
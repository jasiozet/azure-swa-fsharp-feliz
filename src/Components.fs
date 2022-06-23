module App

open Browser.Dom
open Fetch
open Thoth.Fetch
open Feliz


type Components =
  [<ReactComponent>]
  static member Counter () =
      let (count, setCount) = React.useState (0)

      Html.div [
        Html.text count
        Html.button [
          prop.text "Increment"
          prop.onClick (fun _ -> setCount (count + 1))
        ]
      ]

  [<ReactComponent>]
  static member Message () =
    let (message, setMessage) = React.useState ("")

    Html.div [
      Html.button [
        prop.text "Get a message from the API"
        prop.onClick
          (fun _ ->
              promise {
                  let! message =
                      Fetch.get (
                          "/api/HttpTrigger?name=FSharp",
                          headers = [ HttpRequestHeaders.Accept "application/json" ]
                      )

                  setMessage message
                  return ()
              }
              |> ignore) ]
      if message = "" then
          Html.none
      else
          Html.p message ]

  [<ReactComponent>]
  static member App () = React.fragment [ Components.Counter(); Components.Message() ]


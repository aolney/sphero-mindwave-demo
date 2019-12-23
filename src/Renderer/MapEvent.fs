module MapEvent

open Elmish

type Msg =
  | Connect
  | UpdateNeuroskyPort of string
  | UpdateSpheroPort of string
  | Meditation of float

///External event wrapping a message
let mapEvent = Event<Msg>()
///Subscription on external events to bring them into Elmish message queue
let mapEventSubscription initial =
    let sub dispatch =
        let msgSender msg =
            msg
            |> dispatch

        mapEvent.Publish.Add(msgSender)

    Cmd.ofSub sub

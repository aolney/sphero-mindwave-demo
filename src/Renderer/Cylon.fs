module Cylon

open System
open Fable.Core
open Fable.Core.JsInterop

open MapEvent

let inline (!!) x = createObj x
let inline (=>) x y = x ==> y

type [<AllowNullLiteralAttribute>] Robot =
    abstract start : unit -> Robot

type [<AllowNullLiteralAttribute>] Cylon =
    abstract robot  : obj -> Robot

[<Import("*","cylon")>]
let cylon : Cylon = jsNative

[<Emit("every($0,$1)")>]
let every( o : obj, o2:obj ) : unit = jsNative

[<Emit("(1).second()")>]
let oneSecond : obj = jsNative

let Connect neuroskyPort spheroPort =
    let cylonConfig =
        !! [
            "connections" =>
                !! [
                    "neurosky" =>
                        !! [
                            "adaptor" => "neurosky"
                            //can be on any rfcomm as long as channel is 1
                            "port" => neuroskyPort
                        ]
                    "sphero" =>
                        !! [
                            "adaptor" => "sphero"
                            //can be on any rfcomm as long as channel is 1
                            "port" => spheroPort
                        ]
                ]
            "devices" =>
                !! [
                    "headset" =>
                        !! [
                            "driver" => "neurosky"
                        ]
                    "sphero" =>
                        !! [
                            "driver" => "sphero"
                            "connection" => "sphero"
                        ]
                ]
            "work" => fun my -> my?headset?on("meditation", fun (data : int) ->
                Browser.Dom.console.log("meditation:" + data.ToString() )
                let meditation = data |> float
                MapEvent.mapEvent.Trigger ( Meditation meditation)
                if meditation > 70.0 then
                    my?sphero?color("green")
                    my?sphero?roll(60,0);
                else
                    my?sphero?color(
                        !! [
                            "red" => (meditation / 100.0) * 255.0
                            "blue" => (100.0 - meditation / 100.0) * 255.0
                            "green" => 0
                        ]
                    )
                    // my?sphero?stop()
                )
        ]
    cylon.robot(cylonConfig).start() |> ignore
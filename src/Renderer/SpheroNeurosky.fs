module SpheroNeurosky

open System
open Fable.Core
open Fable.Core.JsInterop

let inline (!!) x = createObj x
let inline (=>) x y = x ==> y

type [<AllowNullLiteralAttribute>] Robot =
    abstract start : unit -> Robot

type [<AllowNullLiteralAttribute>] Cylon =
    abstract robot  : obj -> Robot

[<Import("*","cylon")>]
let cylon : Cylon = jsNative


//Not working; babel upset at parsing 'my'?
//[<Emit("function(my) { my.headset.on('meditation',function(data) { Logger.info('meditation:' + data); }); }")>]
//let work() : string = jsNative

//Not working: "Logger not defined"
// [<Emit("Logger.info($0)")>]
// let Log( text : string ): unit = jsNative

let Go() =
    //adapted from
    // http://blog.leapmotion.com/controlling-sphero-leap-motion-cylon-js/
    // https://github.com/hybridgroup/cylon-neurosky
    (*
    function(my){
        my.headset.on('meditation',function(data){
            Logger.info('meditation:' + data);
            my.sphero.roll(60,0);
        });
    }
    *)
    let cylonConfig =
        !! [
            "connections" =>
                !! [
                    "neurosky" =>
                        !! [
                            "adaptor" => "neurosky"
                            "port" => "/dev/rfcomm0"
                        ]
                    "sphero" =>
                        !! [
                            "adaptor" => "sphero"
                            "port" => "/dev/rfcomm1"
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
            "work" => fun my -> my?headset?on("meditation", fun data ->
                Browser.Dom.console.log("meditation:" + data )
                my?sphero?roll(60,0);
                )
        ]
    cylon.robot(cylonConfig).start() |> ignore
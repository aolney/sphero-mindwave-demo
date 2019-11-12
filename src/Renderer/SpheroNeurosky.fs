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

[<Emit("every($0,$1)")>]
let every( o : obj, o2:obj ) : unit = jsNative

[<Emit("(1).second()")>]
let oneSecond : obj = jsNative

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
                            //can be on any rfcomm as long as channel is 1
                            "port" => "/dev/rfcomm1"
                        ]
                    "sphero" =>
                        !! [
                            "adaptor" => "sphero"
                            //can be on any rfcomm as long as channel is 1
                            "port" => "/dev/rfcomm0"
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
                    //my?sphero?stop()
                )
            //This function for testing sphero only
            // "work" => fun my2 ->
            //     my2?sphero?on( "error", fun( err, data) ->
            //         Browser.Dom.console.log( "err:" + err + " data:" + data )
            //     )
            //     every( 1000, fun () ->
            //         //does the function execute?
            //         Browser.Dom.console.log("tick" )
            //         //does the callback return (otherwise we had a serialport error)?
            //         my2?sphero?roll(60,0, fun () ->  Browser.Dom.console.log("roll call" ) );
            //     )
        ]
    cylon.robot(cylonConfig).start() |> ignore
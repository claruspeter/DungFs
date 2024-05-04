open System

Console.Clear()
printfn "Welcome to DungFs!"
printfn "=================="
printfn ""
printfn "What would you like to do?"

let ch = Console.ReadKey(true)
printfn "You chose %A" ch.KeyChar


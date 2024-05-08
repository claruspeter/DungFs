module Tests.Helpers
open DungFs

let maxDice = {rollFunction = fun n -> n}
let minDice = {rollFunction = fun _ -> 1}
let fixedDice n = {rollFunction = fun _ -> n}
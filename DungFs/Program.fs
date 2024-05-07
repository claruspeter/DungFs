open System
open DungFs

Play.standardDice
|> Play.enterDungeon
|> DungFs.Console.gameLoop.start
|> ignore


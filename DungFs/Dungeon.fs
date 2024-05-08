namespace DungFs
open System

type Dungeon = {
  player: Person
  here: Room
  messages: string list
  dice: Dice
  gameOver: bool
}with 
  member this.message = this.messages.Head

module Builder = 
  let withInhabitant person dungeon =
    {
      dungeon with 
        here = dungeon.here |> Rooms.withInhabitant person
    }
  
  let msg m dungeon =
    { dungeon with messages = [m] @ dungeon.messages}

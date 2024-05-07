namespace DungFs
open System

type Dungeon = {
  player: Person
  here: Room
  message: string
  dice: int -> int
  gameOver: bool
}

module Builder = 
  let withInhabitant person dungeon =
    {
      dungeon with 
        here = dungeon.here |> Rooms.withInhabitant person
    }

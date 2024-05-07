module Tests.Rooms

open System
open Xunit
open FsUnit.Xunit
open FsUnitTyped
open DungFs

let maxDice = {rollFunction = fun n -> n}
let minDice = {rollFunction = fun _ -> 1}
let fixedDice n = {rollFunction = fun _ -> n}

let room = Room.empty [North ; West]

[<Fact>]
let ``A room may have exits`` () =
  room.exits.Length |> should equal 2

[<Fact>]
let ``A room may have gold in it`` () =
  let richRoom = {room with gold = 24}
  richRoom.gold |> should equal 24

[<Fact>]
let ``A room may have people in it`` () =
  let crowdedRoom = {room with inhabitant = Some Person.empty}
  crowdedRoom.inhabitant |> shouldNotEqual None

[<Fact>]
let ``A generated room has a name`` () =
  let room = Rooms.generateRoomTo maxDice North
  room.name |> shouldNotEqual ((Room.empty []).name)

[<Fact>]
let ``A generated room has at least one exit`` () =
  let room = Rooms.generateRoomTo maxDice North
  room.exits.Length |> should greaterThan 0

[<Fact>]
let ``A generated room does not have an exit in the direction of the entrance`` () =
  let room = Rooms.generateRoomTo maxDice North
  room.exits |> shouldNotContain South

[<Fact>]
let ``A generated room may have dice determined gold amount`` () =
  let room = Rooms.generateRoomTo maxDice North
  room.gold |> should equal 20

[<Fact>]
let ``A generated room has 50% chance of gold`` () =
  let roomRoll10 = Rooms.generateRoomTo (fixedDice 10) North
  roomRoll10.gold |> should equal 0
  let roomRoll11 = Rooms.generateRoomTo (fixedDice 11) North
  roomRoll11.gold |> should equal 11

[<Fact>]
let ``A generated room has 75% chance of an inhabitant`` () =
  let roomRoll5 = Rooms.generateRoomTo (fixedDice 5) North
  roomRoll5.inhabitant |> should equal None
  let roomRoll6 = Rooms.generateRoomTo (fixedDice 6) North
  roomRoll6.inhabitant |> shouldNotEqual None

[<Fact>]
let ``A generated room has a dice determined inhabitant`` () =
  let roomRoll6 = Rooms.generateRoomTo (fixedDice 6) North
  roomRoll6.inhabitant.Value.name |> should equal "Giant"
  let roomRoll7 = Rooms.generateRoomTo (fixedDice 7) North
  roomRoll7.inhabitant.Value.name |> should equal "Snake"

[<Fact>]
let ``When an inhabitant generated a second time then the health has the default value`` () =
  let dice = fixedDice 6
  let first = Rooms.generateRoomTo dice North
  first.inhabitant.Value.health |> should equal 16

  let dungeon = {(Play.enterDungeon dice) with here = first}
  let dungeonWithDamagedMonster = dungeon |> Play.attack.go
  dungeonWithDamagedMonster.here.inhabitant.Value.health |> should equal 10

  let dungeonWithRegeneratedRoom = dungeonWithDamagedMonster |> Play.goNorth.go
  dungeonWithRegeneratedRoom.here.inhabitant.Value.health |> should equal 16

[<Fact>]
let ``When an inhabitant generated a second time then the inhabitant's gold has the default value`` () =
  let dice = fixedDice 6
  let first = Rooms.generateRoomTo dice North
  first.inhabitant.Value.gold |> should equal 3
  
  let dungeon = {(Play.enterDungeon dice) with here = first}
  let dungeonWithDeadMonster = 
    dungeon 
    |> Play.attack.go
    |> Play.attack.go
    |> Play.attack.go
  dungeonWithDeadMonster.here.inhabitant.Value.gold |> should equal 0  // monster died, so the gold is dropped

  let dungeonWithRegeneratedRoom = dungeonWithDeadMonster |> Play.goNorth.go
  dungeonWithRegeneratedRoom.here.inhabitant.Value.gold |> should equal 3
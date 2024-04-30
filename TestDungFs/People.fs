module Tests.People

open System
open Xunit
open FsUnit.Xunit
open FsUnitTyped
open DungFs

[<Fact>]
let ``A person starts with 100% health`` () =
  let person = Person.empty
  person.health |> should equal 100

[<Fact>]
let ``A person starts alive`` () =
  let person = Person.empty
  person.isDead |> should equal false


[<Fact>]
let ``A person starts with 0 gold`` () =
  let person = Person.empty
  person.gold |> should equal 0

[<Fact>]
let ``When a person picks up gold it is added to their purse`` () =
  let person = Person.empty
  let richPerson = person |> pickupGold 42 
  richPerson.gold |> should equal 42

[<Fact>]
let ``A person can pick up multiple amounts of gold`` () =
  let person = Person.empty
  let richPerson = person |> pickupGold 42 |> pickupGold 69 
  richPerson.gold |> should equal 111

[<Fact>]
let ``When a person can be damaged their health decreases`` () =
  let person = Person.empty
  let hurtPerson = person |> takeDamage 42 
  hurtPerson.health |> should equal 58

[<Fact>]
let ``A person can't be damaged past zero health`` () =
  let person = Person.empty
  let hurtPerson = person |> takeDamage 999 
  hurtPerson.health |> should equal 0

[<Fact>]
let ``When a person's health is zero they are dead`` () =
  let person = Person.empty
  let hurtPerson = person |> takeDamage 100 
  hurtPerson.health |> should equal 0
  hurtPerson.isDead |> should equal true
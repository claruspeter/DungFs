module DungFs
open System

type Person = {
  health: int
  gold: int
} with 
  static member empty = { health = 100; gold = 0}
  member this.isDead = this.health <= 0

let pickupGold amount person =
  { person with gold = person.gold + amount }

let takeDamage amount person =
  { person with health = Math.Max(0, person.health - amount) }

type Direction = N | E | S | W

type Room ={
  exits: Direction list
  gold: int
  people: Person list
} with 
  static member empty exits = {Room.exits = exits; gold=0; people = []}
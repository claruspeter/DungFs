test:
	cd TestDungFs; dotnet watch test  --logger:"console;verbosity=normal"

run:
	cd PlayDungFs; dotnet run

fable:
	cd DungFable; make build
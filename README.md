# Data Tree

## General

A collection of classes representing a tree-shaped data structure that can store different types of data points: Integer, Float, Boolean, Strings, Choices, Binaries.

It is organized in container classes (e.g. `DataContainer`) that can be either statically embedded into other containers or in a more dynamical way like an array of containers (`DataDynContainer`, `DataDynParentContainer`).

## Features

The complete data tree can be saved to and loaded from an XML document.

It supports Undo/Redo functionality.

A generic Data Tree Browser is delivered as a WPF component (see `DataTree.Ui.csproj`).

## Sample tree

To demonstrate the very simple use of the classes the `SampleModel.csproj` project can be inspected.

## Licence

The code is published under MIT license and comes 'as is' with no warranties.

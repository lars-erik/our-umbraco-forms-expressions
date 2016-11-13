# UFX: Our Umbraco Forms Expressions

An expression parser for Umbraco Forms. Can set field values based on arithmetics.


## Installation

Installation is done from nuget.

`install-package our.umbraco.forms.expressions.web`

## Usage

Add a workflow of the type "UFX Calculation" to your form.

Enter assignments and calculations in the "program" setting using the [UFX syntax](wiki/ufx-language).
Assign values to fields.

Click the "full screen" button to open the workbench.

## Using the workbench

The workbench is divided into three areas: the editor, parameters and actions.

Whenever the program changes, the referenced fields appear in the parameter list. Enter values for the read fields. Click the run button to simulate applying the program to a form submission. The set fields and the result of the expression is displayed below the parameters.

## More

- [UFX Syntax](wiki/ufx-language)
- [Builtin functions](wiki/functions)

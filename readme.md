## Our Umbraco Forms Expressions

Currently a prototype in the works.
Can do maths on Umbraco Forms Records and return a value.

Example:

    x = [field value]
    answer = x * 2

Should return 10. :)

### Ambition

Workflow to set field value to result of an equation

To support at least this BNF:

    <program>           ::= <statement> | <statement> <program>

    <statement>         ::= <assignment> | <expression> | <empty>

    <assignment>        ::= <identifier> <equals> <expression>
    <expression>        ::= <term> | <unary expression> | <binary expression>       # possibly introduce if

    <term>              ::= <number> | <group expression> | <field value> | <identifier>
    <unary expression>  ::= <unary operator> <term>
    <binary expression> ::= <expression> <binary operator> <expression>
    <group expression>  ::= "(" <expression> ")"

    <unary operator>    ::= "+" | "-"
    <binary operator>   ::= "+" | "-" | "*" | "/"
    <equals>            ::= "="

    <field value>       ::= "[" <field name> "]"
    <field name>        ::= # name of field, incl. spaces
    <identifier>        ::= # one word identifier, variable

    ! Math imports

    ! Ceiling
    ! Floor
    ! Abs
    ! more?
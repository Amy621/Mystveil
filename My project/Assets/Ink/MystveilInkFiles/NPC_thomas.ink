-> thomas_intro

== thomas_intro ==
Liora: Mr. Thomas!
Thomas: Hmph. Hello.
    + [Ask what he's up to]
        -> whats_up_blacksmith
    + [Ask to buy something]
        -> buy
        
== whats_up_blacksmith ==
Liora: What are you making right now?
Thomas: Commission piece. Broad Sword.
Liora: I see. Good luck!
-> END

== buy ==
{ not empathy or not justice or not courage:
    Liora: Can I buy something?
    Thomas: Come back later. I don't have enough stock right now with the gate closed.
    Liora: Alright...
}
{ empathy and justice and courage:
    Liora: Can I buy something now?
    Thomas: Too busy with commissions.
    Liora: Awww..
}
-> END
    
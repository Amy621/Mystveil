-> henry_intro

== henry_intro ==
{ defend_orphans:
    Liora: Hey Henry.
    Henry: *glasses shine* Liora. I got to get back home and read about a research paper. Goodbye.
    Liora: ... Bye I guess. He's still the same as ever.
}
{ not justice:
    Liora: Hey Henry.
    Henry: Liora. I am extremely busy with my research. If you'll excuse me.
    Liora: Alright, see you!
}
{ go_to_friends_side:
    Liora: Hi Henry!
    Henry: Liora.
    Liora: Wait, you're not going to walk away for once?
    Henry: I have some time right now.
        + [Ask about his research]
            -> research
        + [Ask about the academia]
            ->academia_acceptance
        + [Ask about his parents]
            ->henry_parents
}
-> END

== research ==
Liora: What was your research about again?
Henry: It's about how we can use magic to re-invigorate the human body and become immortal through our own means. As magic is a part of every one of us.
Liora: Huh, that's cool. If we could become immortal then why haven't we?
Henry: Our cells would probably rupture before we could. Also it depends on the magical abilities of the person and what attribute they hold.
Liora: Oh...
-> END

== academia_acceptance ==
Liora: Congrats on getting into the Magic Academia again. 
Henry: Thank you. I am excited to have a place to do my research and practice my magic.
Liora: I hope you don't explode too many flasks!
-> END

== henry_parents ==
Liora: How's your parents doing? Are they supportive to you going to the Academia?
Henry: They are very supportive. Maybe too supportive. I don't know if I am going to be able to fit many books into my luggage and my mother has gifted me 20 textbooks.
Liora: ... That's a bit too much.
Henry: They are all quite interesting though, perhaps I should invest in a magical storage space like you have.
Liora: Aaaand of course you want to bring them with you. I should've know.
-> END
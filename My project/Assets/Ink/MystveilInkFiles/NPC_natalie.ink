-> natalie_intro

== natalie_intro ==
{ defend_orphans:
    Liora: Hey Natalie.
    Natalie: Hey Liora. Hope everything has been well with you. I have to get going.
    Liora: ... see you...
}
{ not justice:
    Liora: Hey Natalie!
    Natalie: Hey! I'm ready with my sweets for the tea party!
    Liora: I love your sweets Natalie! What's the secret?
    Natalie: I put all my happy thoughts into the spell and BAM amazing sweet created!
    Liora: That's actually very convinent.
    Natualie: Sure is!
}
{ go_to_friends_side:
    Liora: Hi Natalie!
    Natalie: Hey Liora! How's it going girl?
        + [Ask how she is]
            -> ask_about_natalie
        + [Leave]
            -> leave_natalie
}
-> END

== ask_about_natalie ==
Liora: How's it going with you?
Natalie: Good, packing my wand and clothes and all the cooking supplies I need to make mountains of cookies at the Magic Academia!
Liora: Sounds like you for sure.
-> END

== leave_natalie ==
Liora: I gotta get going now. Nice talking to you!
Natalie: Same, bye!
-> END
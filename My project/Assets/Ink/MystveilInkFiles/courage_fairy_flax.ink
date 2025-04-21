-> get_fairy_flax

== get_fairy_flax ==
Liora: I'm back!
Gale: Better late then never, shortie. Just bring it over here.
Liora: Ugh, fine.
Gale: Okay now I can tell you where that furball is- he's been here the whole time!
Liora: WHAT!! You trickster!
Gale: Hehe! That's what you get :P
    + [Attack him now!!]
        -> attack
    + [Accept that you were beat]
        -> acceptance

== acceptance ==
~ player_charisma += 1
{ get_fairy_flax:
    Liora: Ugh, you win again I guess.
    Gale: Yippee! Now I can go and cause chaos in town :D Bye bye!
    Liora: ...
    Hecuba: Well that might backfire in the near future.
    Liora: Hecuba! You know that he causes trouble, so why did you let him go?
    Hecuba: He needs to get some fun once in a while. It's harmless.
    Liora: Sure doesn't look like it...
    Hecuba: So why are you looking for me?
}
{ battled_gale and (not get_fairy_flax):
    Hecuba: Just go and do your chores, Gale.
    Gale: Fine...
    Liora: He's finally gone! Thanks Hecuba.
    Hecuba: Your welcome, child. Now what brings you here?
}

Liora: I was wondering if you have seen a gemstone flying around here.
Hecuba: Flying gemstone? I can't say I have. I did see a strange light in your mother's room.
Liora: Mother's room? Maybe I can ask her about it.
Hecuba: Oh no young one, she's extremely busy at this time of year. Don't go intruding on her.
    + [Argue that you need to find it]
        -> argue
    + [Ask if you can help]
        -> help_hecate

== argue ==
~ player_charisma -= 1
Liora: Just let me see her! I haven't seen her in a couple months- she's been too cooped up in her room.
Hecuba: No. 
Liora: Come on, please??
Hecuba: I will not budge.
    + [Battle]
        -> pre_hecuba_battle
    + [Bribe]
        -> bribe_hecuba

== help_hecate ==
~ player_charisma += 2
Liora: Maybe there's some way I can help? If her workload is less then I can ask! I've went into the forest many times to find the ingredients- shouldn't be any different.
Hecuba: Hmm if you so wish, then find a couple of these herbs for her. And this.
Liora: Alright, I'll be back soon!
Hecuba: ... If she manages to bring back that item, then maybe she is ready.
-> END

== pre_hecuba_battle ==
Hecuba: You will never be ready, child.
-> END

== bribe_hecuba ==
~ player_charisma -= 1
Hecuba: You can't sway me unless you show your true strength.
-> END
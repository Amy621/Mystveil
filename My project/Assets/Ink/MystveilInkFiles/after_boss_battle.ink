-> after_boss_battle

== after_boss_battle ==
{ player_charisma >= 10:
    Circe: Urgh..
    Liora: I've defeated you. Leave.
    Circe: Even if I leave, there is the entire Royal Guard and the Royal family who will appoint the next successor. You will never be chosen!
    Liora: Even if I'm not chosen, I'll rise the ranks towards becoming The Court Magician. Just watch!
    Circe: ... Your passion mirrors mine from when I first joined. Where did all of that courage, empathy, and justice go? Perhaps you were always meant to best me in a duel.
    Liora: ...
    -> END
}
{ player_charisma < 10:
    Circe: Urgh... You haven't defeated me yet. I can sense your cowardice. You cannot withstand this spell-!
    Liora: What's with that light?!
    Liora: ...
    Circe: My magic will make you forget this encounter. Now go and join The Royal Guard... and become my subordinate.
    -> END
}
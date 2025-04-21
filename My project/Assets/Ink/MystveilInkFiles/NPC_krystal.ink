-> krystal_intro

== krystal_intro ==
Liora: Hello Ms. Krystal!
Krystal: Kekeke... Hello Little Liora.
    + [Ask about her progress]
        -> finding_the_shadow_mage
    + [Ask for your fortune]
        -> ask_about_fortune

== finding_the_shadow_mage ==
Liora: I know that you came to Mystveil in search for The Shadow Mage. How's the search going?
Krystal: Not well. She always sneaks out of my grasp. Even with my clear crsytal ball that reveals all- she hides in the shadows where it cannot reach.
Liora: Oh no. I hope you find her soon. Why are you trying to find her again?
Krystal: I guess you can consider her a rival of mine.
Liora: Well, wishing you luck on your search!
-> END

== ask_about_fortune ==
Liora: Can you tell my fortune today?
Krystal: Since you're always a good girl, I'll give you a free divination. Let's see...
{ not empathy and not courage and not justice:
    Krystal: The three veils have not been cast off just yet, but come time and the true path will reveal itself to you.
}
{ (empathy and courage) or (empathy and justice) or (justice and courage):
    Krystal: A path forward is visible, but you are missing the final impetus for stepping foward. Once you find it, you will evolve leaps and bounds towards the future you seek.
}
{ empathy and courage and justice:
    Krystal: The journey isn't about the destination- but about the path you took to get to where you are now.
}
-> END
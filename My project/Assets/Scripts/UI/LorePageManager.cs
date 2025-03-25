using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class LorePage
{
    public string title;
    public string content;
}

public class LorePageManager : MonoBehaviour
{
    [Header("Page Content")]
    public List<LorePage> pages = new List<LorePage>();
    public int currentPageIndex = 0;

    [Header("UI References")]
    public TMP_Text leftPageTitle;
    public TMP_Text leftPageContent;
    public TMP_Text rightPageTitle;
    public TMP_Text rightPageContent;
    
    [Header("Navigation")]
    public Button previousButton;
    public Button nextButton;
    public TMP_Text pageNumbers;
    
    [Header("Page Turn")]
    public Animator pageTurnAnimator;
    public AudioSource pageTurnSound;
    
   void Start()
{
    AddLorePage(
        "Mystveil",
        "Mystveil is a gloomy town beside the pristine Leifdreina, capital city of the continent. Most travelers that travel to the capital city opt to skip visiting Mystveil due to the rumors circling around. That Mystveil is cursed and forever swarmed with monsters of every size and shape. The children of Mystveil often leave to pursue their dreams of becoming guards of The Leifdreina Royal Guard."
    );
    
    AddLorePage(
        "Leifdreina",
        @"Leifdreina is the esteemed capital city of the continent. People travel from all over to visit.
The people are protected by The Royal Guard and led by Ihe Court Magician- the strongest witch in the land.
Once a month, fights are held in the Gladiator's Arena and people who disobey the laws fight for their lives while being swarmed by a sea of monsters."
    );
    AddLorePage(
        "The Royal Guard",
        "The Royal Guard is the elite force of the Leifdreina Kingdom, tasked with protecting the royal family and maintaining order in the capital. They are known for their exceptional combat skills and magical prowess. The Court Magician, who leads the guard, is considered the most powerful witch in the land."
    );
    AddLorePage(
        "The Court Magician",
        "The Court Magician is the most powerful witch in the Leifdreina Kingdom. They are responsible for advising the royal family and using their magic to protect the kingdom from threats. The current Court Magician is a mysterious figure, known only by their title, and is rumored to have a deep connection with the magical creatures of Mystveil."
    );
    AddLorePage(
        "The Villagers",
        "The villagers of Mystveil are a hardy and resilient people. They have learned to adapt to the dangers of their surroundings, living in harmony with the monsters that inhabit the area. Many villagers are skilled in magic and combat, using their abilities to protect their homes and families. Despite the challenges they face, they remain hopeful for a brighter future."
    );
    AddLorePage(
        "The Monsters",
        "The monsters of Mystveil are a diverse and dangerous bunch. They range from small, mischievous creatures to large, terrifying beasts. Some are known to guard treasures, while others roam the forests and rivers, posing a threat to unsuspecting travelers. The villagers have learned to coexist with these creatures, but caution is always advised."
    );
    AddLorePage(
        "Liora's Journey",
        "Liora is a newbie witch who wishes to join the Royal Guard as the Court Magician. Battle monsters, solve problems, and finally unlock the way to the capital. As the player follows Liora, they can choose whether they experience a coming-of-age story with Liora shifting from a materialistic view to a more encompassing understanding of the world, or have her remain true to her original goal. (More about Liora in the Characters doc.)"
    );
    
    AddLorePage(
        "Kokedama Forest",
        "Kokedama Forest: Surrounding Mystveil, this forest is home to plant-like monsters and eerie woodland creatures such as moss ball slimes, tree-like beings, mushroom monsters, and mutant squirrels.\n\nRapid Rivers: Located east of the town, these rivers provide clean water to Mystveil. However, villagers avoid crossing because the deeper areas hide monsters and the current grows dangerously strong."
    );
    
    
    
    // Set up button listeners
    previousButton.onClick.AddListener(PreviousPages);
    nextButton.onClick.AddListener(NextPages);
    
    // Initialize first pages
    UpdatePages();
}
    
    void UpdatePages()
    {
        // Update left page
        if (currentPageIndex < pages.Count)
        {
            leftPageTitle.text = pages[currentPageIndex].title;
            leftPageContent.text = pages[currentPageIndex].content;
        }
        
        // Update right page
        if (currentPageIndex + 1 < pages.Count)
        {
            rightPageTitle.text = pages[currentPageIndex + 1].title;
            rightPageContent.text = pages[currentPageIndex + 1].content;
        }
        else
        {
            // Clear right page if we're at the end
            rightPageTitle.text = "";
            rightPageContent.text = "";
        }
        
        // Update page numbers
        pageNumbers.text = $"Pages {currentPageIndex + 1}-{Mathf.Min(currentPageIndex + 2, pages.Count)} of {pages.Count}";
        
        // Update button interactability
        previousButton.interactable = currentPageIndex > 0;
        nextButton.interactable = currentPageIndex + 2 < pages.Count;
    }
    
    public void NextPages()
    {
        if (currentPageIndex + 2 < pages.Count)
        {
            if (pageTurnAnimator != null)
            {
                pageTurnAnimator.Play("TurnPageForward");
            }
            
            if (pageTurnSound != null)
            {
                pageTurnSound.Play();
            }
            
            currentPageIndex += 2;
            UpdatePages();
        }
    }
    
    public void PreviousPages()
    {
        if (currentPageIndex > 0)
        {
            if (pageTurnAnimator != null)
            {
                pageTurnAnimator.Play("TurnPageBackward");
            }
            
            if (pageTurnSound != null)
            {
                pageTurnSound.Play();
            }
            
            currentPageIndex -= 2;
            UpdatePages();
        }
    }
    
    // Method to add new lore pages
    public void AddLorePage(string title, string content)
    {
        LorePage newPage = new LorePage
        {
            title = title,
            content = content
        };
        pages.Add(newPage);
    }
} 
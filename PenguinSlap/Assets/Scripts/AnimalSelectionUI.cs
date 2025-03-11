using UnityEngine;
using TMPro;

public class AnimalSelectionUI : MonoBehaviour
{
    public TextMeshProUGUI penguinText;
    public TextMeshProUGUI tigerText;
    public TextMeshProUGUI horseText;

    private string[] penguinTypes = { "Penguin Banana", "Penguin Wine", "Penguin Burger" };
    private string[] tigerTypes = { "Tiger Eggplant", "Tiger Beer", "Tiger Sushi" };
    private string[] horseTypes = { "Horse Donut", "Horse Carrot", "Horse Tomato" };

    void Start()
    {
        AssignRandomAnimals();
    }

    private void AssignRandomAnimals()
{
    string selectedPenguin = penguinTypes[Random.Range(0, penguinTypes.Length)];
    string selectedTiger = tigerTypes[Random.Range(0, tigerTypes.Length)];
    string selectedHorse = horseTypes[Random.Range(0, horseTypes.Length)];

    penguinText.text = selectedPenguin;
    tigerText.text = selectedTiger;
    horseText.text = selectedHorse;

    selectedPenguin = selectedPenguin.Replace(" ", "");
    selectedTiger = selectedTiger.Replace(" ", "");
    selectedHorse = selectedHorse.Replace(" ", "");
    
    // Store the assigned animals in ScoreManager
    ScoreManager.Instance.AssignAnimals(selectedPenguin, selectedTiger, selectedHorse);
}

}

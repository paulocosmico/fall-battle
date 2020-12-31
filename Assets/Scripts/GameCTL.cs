﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class GameCTL : MonoBehaviour
{
    private GridCTL _grid;
    //cards de teste
    [SerializeField] private List<Card> _listOfAllCards;
    public List<Card> GetListOfAllCards(){
        return _listOfAllCards;
    }
    #region SINGLETON
    private static GameCTL _instance;
    public static GameCTL Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameCTL>();
            }

            return _instance;
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    #endregion
    void Start()
    {
        _grid = GameObject.FindGameObjectWithTag("grid").GetComponent<GridCTL>();
        _listOfAllCards = new List<Card>();
        ReadData("/cards.tsv");
    }
    public Card PickACardInListOfAllCards(bool randomCard, int id = -1){
        if(randomCard){
            var random = new System.Random();
            int index = random.Next(_listOfAllCards.Count);
            return _listOfAllCards[index];
        }else{
            try
            {
                int index = _listOfAllCards.FindIndex(c => c.GetId() == id);
                return _listOfAllCards[index];
            }
            catch (System.Exception)
            {
                Debug.LogError("erro acess card ID");
                throw;
            }
            
        }
    }
    //TODO:: CAMINHO Trilha_Data/StreamingAssets/   Assets/Data/
    private void ReadData(string filePath){
        using(var reader = new StreamReader(Application.streamingAssetsPath+filePath))
        {
            //tsv columns
            //0-name	1-kingdom	2-card_type	3-cost_mana	4-unit_type	
            //5-hp	  6-atk_range	  7-atk_damage	8-atk_speed	
            //9-heal_power	 10-heal_range	  11-heal_speed	
            //12-respawn_cooldown	13-move_speed
            int countId = 0;
            bool head = true;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split('\t');
                if(!head){
                    Card card = new Card(countId,
                                    values[0],
                                    values[1],
                                    values[2],
                                    int.Parse(values[12]),
                                    int.Parse(values[3]),
                                    values[4],
                                    int.Parse(values[5]),
                                    new RangeTiles(values[6]),
                                    int.Parse(values[7]),
                                    int.Parse(values[8]),
                                    int.Parse(values[9]),
                                    new RangeTiles(values[10]),
                                    int.Parse(values[11]),
                                    int.Parse(values[13])); 
                    _listOfAllCards.Add(card);
                    countId++;
                }
                head = false;
            }
        }
    }
    //execute action card
    public void UseCard(Card card){
        if(PlayerCTL.Instance.GetTargetTile() != null){
            switch (card.GetCardType())
            {
                case "unit":
                    if(!PlayerCTL.Instance.GetTargetTile().GetUnit().isActiveAndEnabled){
                       PlayerCTL.Instance.GetTargetTile().GetUnit().SetingUnitFromThePLayer(card);
                        card.gameObject.SetActive(false); 
                    }
                    break;
                default:
                    break;
            }
        }
    }
}

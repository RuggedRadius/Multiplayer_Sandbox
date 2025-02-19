using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mirror.Examples.Tanks
{
    public class MMOGameManager : MonoBehaviour
    {
        public static MMOGameManager singleton;

        public int MinimumPlayersForGame = 0;

        public Player LocalPlayer;
        public GameObject MainMenuSprite;
        public GameObject StartPanel;
        public GameObject GameOverPanel;
        public GameObject HealthTextLabel;
        public GameObject ScoreTextLabel;
        public Text HealthText;
        public Text ScoreText;
        public Text PlayerNameText;
        public Text WinnerNameText;
        public bool IsGameReady;
        public bool IsGameOver;
        public List<Player> players = new List<Player>();

        public WorldFiller worldFiller;

        private void Awake()
        {
            if (singleton == null)
                singleton = this;

            if (singleton != null && singleton != this)
                Destroy(this.gameObject);
        }

        private void Start()
        {
            MapGenerator mapGen = FindObjectOfType<MapGenerator>();
            mapGen.seed = Random.Range(int.MinValue, int.MaxValue);
            mapGen.GenerateMap();

            //StartCoroutine(worldFiller.FillWorld());
        }

        void Update()
        {

            if (Input.GetKeyDown(KeyCode.L))
            {
                Debug.Log("Filling world test");
                worldFiller.Fill();
            }



            if (NetworkManager.singleton.isNetworkActive)
            {
                GameReadyCheck();
                GameOverCheck();

                if (LocalPlayer == null)
                {
                    FindLocalTank();
                }
                else
                {
                    ShowReadyMenu();
                    //UpdateStats();
                }
            }
            else
            {
                //Cleanup state once network goes offline
                IsGameReady = false;
                LocalPlayer = null;
                players.Clear();
            }
        }

        void ShowReadyMenu()
        {
            if (NetworkManager.singleton.mode == NetworkManagerMode.ServerOnly)
                return;

            if (LocalPlayer.isReady)
                return;

            StartPanel.SetActive(true);
        }

        void GameReadyCheck()
        {
            if (!IsGameReady)
            {
                //Look for connections that are not in the player list
                foreach (KeyValuePair<uint, NetworkIdentity> kvp in NetworkIdentity.spawned)
                {
                    Player comp = kvp.Value.GetComponent<Player>();

                    //Add if new
                    if (comp != null && !players.Contains(comp))
                    {
                        players.Add(comp);
                    }
                }

                //If minimum connections has been check if they are all ready
                if (players.Count >= MinimumPlayersForGame)
                {
                    bool AllReady = true;
                    foreach (Player player in players)
                    {
                        if (!player.isReady)
                        {
                            AllReady = false;
                        }
                    }
                    if (AllReady)
                    {
                        IsGameReady = true;                        
                        AllowTankMovement();

                        //Update Local GUI:
                        StartPanel.SetActive(false);                        
                        HealthTextLabel.SetActive(true);
                        ScoreTextLabel.SetActive(true);
                    }
                }
            }
        }

        void GameOverCheck()
        {
            //if (MainMenuSprite.activeSelf)
            //    MainMenuSprite.SetActive(false);

            if (!IsGameReady)
                return;

            //Cant win a game you play by yourself. But you can still use this example for testing network/movement
            if (players.Count == 1)
                return;

            int alivePlayerCount = 0;
            foreach (Player player in players)
            {
                if (!player.isDead)
                {
                    alivePlayerCount++;

                    //If there is only 1 player left alive this will end up being their name
                    WinnerNameText.text = player.playerName;
                }
            }

            if (alivePlayerCount == 1)
            {
                IsGameOver = true;
                GameOverPanel.SetActive(true);
                DisallowTankMovement();
            }
        }

        void FindLocalTank()
        {
            //Check to see if the player is loaded in yet
            if (ClientScene.localPlayer == null)
                return;

            LocalPlayer = ClientScene.localPlayer.GetComponent<Player>();
        }

        void UpdateStats()
        {
            HealthText.text = LocalPlayer.health.ToString();
            ScoreText.text = LocalPlayer.score.ToString();
        }

        public void ReadyButtonHandler()
        {
            LocalPlayer.SendReadyToServer(PlayerNameText.text);
        }

        //All players are ready and game has started. Allow players to move.
        void AllowTankMovement()
        {
            foreach (Player player in players)
            {
                player.allowMovement = true;
            }
        }

        //Game is over. Prevent movement
        void DisallowTankMovement()
        {
            foreach (Player player in players)
            {
                player.allowMovement = false;
            }
        }
    }
}

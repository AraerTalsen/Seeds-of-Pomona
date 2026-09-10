using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoonDisplay
{
    [System.Serializable]
    public class BoonDisplayProps
    {
        [SerializeField] private Image speedImg;
        [SerializeField] private Color speedColor;
        [SerializeField] private TextMeshProUGUI speedText;
        [SerializeField] private Image strengthImg;
        [SerializeField] private Color strengthColor;
        [SerializeField] private TextMeshProUGUI strengthText;
        [SerializeField] private Image healthImg;
        [SerializeField] private Color healthColor;
        [SerializeField] private TextMeshProUGUI healthText;

        public Image SpeedImg { get => speedImg; set => speedImg = value; }
        public Image StrengthImg { get => strengthImg; set => strengthImg = value; }
        public Image HealthImg { get => healthImg; set => healthImg = value; }
        public Color SpeedColor { get => speedColor; set => speedColor = value; }
        public Color StrengthColor { get => strengthColor; set => strengthColor = value; }
        public Color HealthColor { get => healthColor; set => healthColor = value; }
        public TextMeshProUGUI SpeedText { get => speedText; set => speedText = value; }
        public TextMeshProUGUI StrengthText { get => strengthText; set => strengthText = value; }
        public TextMeshProUGUI HealthText { get => healthText; set => healthText = value; }
    }

    private BoonDisplayProps boonDisplayProps;
    
    public BoonDisplay(BoonDisplayProps props) => boonDisplayProps = props;
    
    public void UpdateStatDisplay(Stats stat, int amount)
    {
        if(amount > 0)
        {
            switch(stat)
            {
                case Stats.Speed:
                {
                    boonDisplayProps.SpeedImg.color = boonDisplayProps.SpeedColor;
                    boonDisplayProps.SpeedText.text = amount.ToString();
                    break;
                }
                case Stats.Strength:
                {
                    boonDisplayProps.StrengthImg.color = boonDisplayProps.StrengthColor;
                    boonDisplayProps.StrengthText.text = amount.ToString();
                    break;
                }
                case Stats.Health:
                {
                    boonDisplayProps.HealthImg.color = boonDisplayProps.HealthColor;
                    boonDisplayProps.HealthText.text = amount.ToString();
                    break;
                }
            }   
        }
    }
}

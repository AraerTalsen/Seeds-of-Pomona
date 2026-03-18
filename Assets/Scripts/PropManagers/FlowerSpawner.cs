using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerSpawner : ProceduralGenerator
{
    [System.Serializable]
    public class BoundBox
    {
        [SerializeField] private Vector2 topLeft;
        public Vector2 TopLeft => topLeft;
        [SerializeField] private Vector2 botRight;
        public Vector2 BotRight => botRight;

        public BoundBox(Vector2 topLeft, Vector2 botRight)
        {
            this.topLeft = topLeft;
            this.botRight = botRight;
        }

        public void Deconstruct(out Vector2 topLeft, out Vector2 botRight)
        {
            topLeft = TopLeft;
            botRight = BotRight;
        }
    }

    [SerializeField] private List<BoundBox> boundBoxes;
    public List<BoundBox> BoundBoxes => boundBoxes;

    // Start is called before the first frame update
    void Start()
    {
        foreach(BoundBox box in boundBoxes)
        {
            width = (int)(box.BotRight.x - box.TopLeft.x);
            height = (int)(box.BotRight.y - box.TopLeft.y);
            origin = new Vector2(box.BotRight.x - width / 2, box.BotRight.y - height / 2);

            GenerateProps();
        }
    }
}

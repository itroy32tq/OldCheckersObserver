using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Checkers
{
    public class CreateManager : MonoBehaviour
    {
        [Tooltip("образец для клетки")]
        [SerializeField]
        private GameObject _referenceCell;

        [Tooltip("образец для фишки")]
        [SerializeField]
        private GameObject _referenceChips;

        [SerializeField]
        private Material[] _cellMaterials = new Material[2];

        [SerializeField]
        private Material[] _shipsMaterials = new Material[2];

        private readonly string[] _word = { "A", "B", "C", "D", "E", "F", "G", "H" };
        // Start is called before the first frame update
        void Start()
        {
            for (int i = 0; i <= 7; i++)
            {
                for (int j = 0; j <= 7; j++)
                {
                    GameObject cell = Instantiate(_referenceCell, new Vector3(i, 0, j), Quaternion.identity);
                    cell.name = _word[j] + (i+1).ToString();
                    CellComponent new_cell = cell.GetComponent<CellComponent>();
                    GameManager.Self._cells[i,j] = new_cell;
                    new_cell.Pair = null;

                    new_cell.Coordinate = new Coordinate(i, j);

                    if (i % 2 == 0)
                        if (j % 2 == 0)
                        {
                            cell.GetComponent<MeshRenderer>().material = _cellMaterials[0];
                            new_cell.Color = ColorType.Black;
                            if (i >= 0 && i <= 2)
                            {
                                GameObject chip_form = Instantiate(_referenceChips, new Vector3(i, 0, j), Quaternion.identity);
                                ChipComponent new_chip = chip_form.GetComponent<ChipComponent>();
                                GameManager.Self._ships.Add(new_chip);
                                new_chip.Pair = new_cell;
                                new_cell.Pair = new_chip;
                                chip_form.name = "BlackChip";
                                chip_form.GetComponent<MeshRenderer>().material = _shipsMaterials[0];
                                new_chip.Color = ColorType.Black;
                                new_chip._chipForm = chip_form;
                            }
                            if (i >= 6 && i <= 8)
                            {
                                GameObject chip_form = Instantiate(_referenceChips, new Vector3(i, 0, j), Quaternion.identity);
                                ChipComponent new_chip = chip_form.GetComponent<ChipComponent>();
                                GameManager.Self._ships.Add(new_chip);
                                new_chip.Pair = new_cell;
                                new_cell.Pair = new_chip;
                                chip_form.name = "WhiteChip";
                                chip_form.GetComponent<MeshRenderer>().material = _shipsMaterials[1];
                                new_chip.Color = ColorType.White;
                                new_chip._chipForm = chip_form;
                            }

                        }
                        else
                        {
                            cell.GetComponent<MeshRenderer>().material = _cellMaterials[1];
                            new_cell.Color = ColorType.White;
                        }

                    else
                         if (j % 2 == 0)
                            {
                                cell.GetComponent<MeshRenderer>().material = _cellMaterials[1];
                                new_cell.Color = ColorType.White;
                            }
                            else 
                            {
                                cell.GetComponent<MeshRenderer>().material = _cellMaterials[0];
                                new_cell.Color = ColorType.Black;
                                if (i >= 0 && i <= 2)
                                {
                                    GameObject chip_form = Instantiate(_referenceChips, new Vector3(i, 0, j), Quaternion.identity);
                                    ChipComponent new_chip = chip_form.GetComponent<ChipComponent>();
                                    GameManager.Self._ships.Add(new_chip);
                                    new_chip.Pair = new_cell;
                                    new_cell.Pair = new_chip;
                                    chip_form.name = "BlackChip";
                                    chip_form.GetComponent<MeshRenderer>().material = _shipsMaterials[0];
                                    new_chip.Color = ColorType.Black;
                                    new_chip._chipForm = chip_form;
                                }
                                if (i >= 5 && i <= 7)
                                {
                                    GameObject chip_form = Instantiate(_referenceChips, new Vector3(i, 0, j), Quaternion.identity);
                                    ChipComponent new_chip = chip_form.GetComponent<ChipComponent>();
                                    GameManager.Self._ships.Add(new_chip);
                                    new_chip.Pair = new_cell;
                                    new_cell.Pair = new_chip;
                                    chip_form.name = "WhiteChip";
                                    chip_form.GetComponent<MeshRenderer>().material = _shipsMaterials[1];
                                    new_chip.Color = ColorType.White;
                                    new_chip._chipForm = chip_form;
                                }
                            }                    
                }
            }
        }
    }
}


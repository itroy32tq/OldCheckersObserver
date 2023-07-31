using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEditor.SceneView;

namespace Checkers
{

    public class GameManager : MonoBehaviour, ISerializable
    {
        public static GameManager Self { get; private set; }

        public List<ChipComponent> _ships = new List<ChipComponent>();

        public CellComponent[,] _cells = new CellComponent[8,8];

        private ChipComponent _chosenChip;
        private CellComponent _chosenCell;
        private ChipComponent _eatenChip;

        [Tooltip("Материал для подсветки при наведении курсора на клетку/выборе фишки"), SerializeField]
        protected Material _onPointerMaterial;

        [Tooltip("Материал для подсветки клетки, на которую можно передвинуть фишку"), SerializeField]
        protected Material _onClickMaterial;

        private readonly string[] _word = { "A", "B", "C", "D", "E", "F", "G", "H" };

        [SerializeField] private CameraMover _cameraMover;
        [SerializeField] private CreateManager _createManager;

        [SerializeField]
        private float _speedMoveChip;

        [SerializeField]
        public bool _isNotBlock = true;

        [SerializeField]
        public ColorType Turn = ColorType.White;

        [SerializeField]
        public Vector3 StartPosition = new Vector3();

        [SerializeField]
        public Vector3 EndPosition = new Vector3();

        [SerializeField]
        public Quaternion StartRotation = new Quaternion();

        [SerializeField]
        public Quaternion EndRotation = new Quaternion();

        private IObservable _observer;

        public event Action<BaseClickComponent> ChipDestroyed;

        public event Action ObjectsMoved;

        public event Action<ColorType> GameEnded;

        public event Action StepOver;

        private void Awake()
        {
            Self = this;
        }
        void Start()
        {
            _chosenChip = null;
            _chosenCell = null;

            StartPosition = new Vector3(12f, 4.5f, 3.5f);
            EndPosition = new Vector3(-5f, 4.5f, 3.5f);
            StartRotation = new Quaternion(0.0922959298f, -0.701057374f, 0.0922959298f, 0.701057374f);
            EndRotation = new Quaternion(0.0922959298f, 0.701057374f, -0.0922959298f, 0.701057374f);

            if (TryGetComponent(out _observer))
            {
                _observer.NextStepReady += OnNextMove;
            }

            foreach (ChipComponent ship in _ships)
            {
                ship.OnFocusEventHandler += ShipPointerHendler;
                ship.OnClickEventHandler += ShipClickHendler;

            }
            
            //ниже конечно редкий изварт, но я по другому не придумал
            for (int i = 0; i <= 7; i++)
            {
                for (int j = 0; j <= 7; j++)
                {
                    _cells[i,j].OnFocusEventHandler += CellPointerHendler;
                    _cells[i,j].OnClickEventHandler += CellClickHendler;

                    _cells[i, j].Coordinate.X = i;
                    _cells[i, j].Coordinate.Y = j;

                    Dictionary<NeighborType, CellComponent> neighbors = new Dictionary<NeighborType, CellComponent>();
                    if (i >= 1 && j < 7) 
                    {
                        neighbors[NeighborType.BottomRight] = _cells[i - 1, j + 1];
                    }
                    if (i < 7 && j < 7) 
                    {
                        neighbors[NeighborType.TopRight] = _cells[i + 1, j + 1];
                    }
                    if (i >= 1 && j >= 1)
                    {
                        neighbors[NeighborType.BottomLeft] = _cells[i - 1, j - 1];
                    }
                    if (i < 7 && j >= 1)
                    {
                        neighbors[NeighborType.TopLeft] = _cells[i + 1, j - 1];
                    } 
                    _cells[i, j].Configuration(neighbors);
                }
            }
        }
        private void OnDisable()
        {
            foreach (var cell in _cells)
            {
                cell.OnFocusEventHandler -= CellPointerHendler;
                cell.OnClickEventHandler -= CellClickHendler;
            }

            foreach (ChipComponent ship in _ships)
            {
                ship.OnFocusEventHandler -= ShipPointerHendler;
                ship.OnClickEventHandler -= ShipClickHendler;

            }
        }

        private async void СhangeTurn()
        {
            await Task.Delay(TimeSpan.FromSeconds(_speedMoveChip));


            if (Turn == ColorType.White)
            {
                Turn = ColorType.Black;
                //StartCoroutine(_cameraMover.Move());
                StartCoroutine(MoveCamera(StartPosition, EndPosition, StartRotation, EndRotation, 1f));
            }

            else
            {
                Turn = ColorType.White;
                //StartCoroutine(_cameraMover.Move());
                StartCoroutine(MoveCamera(EndPosition, StartPosition, EndRotation, StartRotation, 1f));
            } 
        }
        private void CellPointerHendler(CellComponent target, bool isSelect)
        {
            if (isSelect) target.ChengeMaterial(_onPointerMaterial);
            else target.ResetMaterial();
        }

        private void OnNextMove(Coordinate target)
        {
            if (target.X == -1)
            {
                StepOver?.Invoke();
                return;
            }

            
            var targetCell = _cells[target.X, target.Y];

            if (targetCell.Pair != null) ShipClickHendler(targetCell.Pair);
            else CellClickHendler(targetCell);
        }

        private void CellClickHendler(BaseClickComponent target)
        {
            target.ChengeMaterial(_onClickMaterial);

            if (_chosenCell != null && _chosenChip != null & _isNotBlock)
            {
                _observer?.Serialize(_chosenChip.Pair.Coordinate.ToString().ToSerializable(RecordType.Move, target.Coordinate.ToString()));
                //логика сбивания фишки
                if (_chosenCell.GetAviableDown().Contains(target))
                {
                    
                    _isNotBlock = false;
                    _chosenCell.Pair = null;
                    _chosenChip.StartCoroutine(_chosenChip.MoveFromTo(_chosenCell.GetPosition, target.GetPosition, _speedMoveChip));
                    _eatenChip = GetEatenChip(_chosenCell, target);
                    DestroyChip(_eatenChip);
                    _observer?.Serialize(_eatenChip.Pair.Coordinate.ToString().ToSerializable(RecordType.Remove));
                    ChipDestroyed?.Invoke(_eatenChip);
                    _chosenChip.Pair = target;
                    target.Pair = _chosenChip;
                }
                //логика передвижения фишки
                if (_chosenCell.GetAviableMoves().Contains(target))
                {
                    _isNotBlock = false;
                    _chosenCell.Pair = null;
                    _chosenChip.StartCoroutine(_chosenChip.MoveFromTo(_chosenCell.GetPosition, target.GetPosition, _speedMoveChip));
                    _chosenChip.Pair = target;
                    target.Pair = _chosenChip;
                }

                ObjectsMoved?.Invoke();
                StepOver?.Invoke();
                СhangeTurn();
            }   
        }
        private ChipComponent GetEatenChip(BaseClickComponent c1, BaseClickComponent c2)
        {
            var d = c1.GetPosition + (c2.GetPosition - c1.GetPosition) / 2;
            foreach (ChipComponent chip in _ships)
            {
                if (chip.GetPosition == d)
                {
                    return chip;
                } 
            }
            return null;
        }
        private async void DestroyChip(ChipComponent chip)
        {
            await Task.Run(() => Thread.Sleep((int)(_speedMoveChip * 1000)));
            Destroy(chip._chipForm);
            _ships.Remove(chip);
        }
        private void ShipClickHendler(BaseClickComponent target)
        {
            

            if (_isNotBlock && target.Color == GameManager.Self.Turn)
            {
                _observer?.Serialize(target.Pair.Coordinate.ToString().ToSerializable(RecordType.Click));
                if (_chosenChip != null)
                {
                    _chosenChip.ResetMaterial();
                }
                _chosenChip = (ChipComponent)target;
                _chosenCell = (CellComponent)_chosenChip.Pair;
                target.ChengeMaterial(_onClickMaterial);
                
            }
            else Debug.Log("выбрана фишка ирока который не ходит");
            StepOver?.Invoke();
        }


        private void ShipPointerHendler(CellComponent target, bool isSelect)
        {
            if (_isNotBlock && target.Pair.Color == Turn)
            {
                if (isSelect)
                {
                    foreach (CellComponent cell in target.GetAviableMoves())
                    {
                        cell.ChengeMaterial(_onPointerMaterial);
                    }
                    foreach (CellComponent cell in target.GetAviableDown())
                    {
                        cell.ChengeMaterial(_onPointerMaterial);
                    }
                }
                else
                {
                    foreach (CellComponent cell in target.GetAviableMoves())
                    {
                        cell.ResetMaterial();
                    }
                    foreach (CellComponent cell in target.GetAviableDown())
                    {
                        cell.ResetMaterial();
                    }
                }     
            }
        }



        private IEnumerator MoveCamera(Vector3 startPosition, Vector3 endPosition, Quaternion startRotation, Quaternion endRotation, float time)
        {

            float currentTime = 0f;
            //Camera camera = Camera.main;

            while (currentTime < time)
            {
                transform.position = Vector3.Lerp(startPosition, endPosition, 1 - (time - currentTime) / time);
                transform.rotation = Quaternion.Lerp(startRotation, endRotation, 1 - (time - currentTime) / time);
                currentTime += Time.deltaTime;
                yield return null;
            }
            transform.position = endPosition;
            transform.rotation = endRotation;
            _isNotBlock = true;
            yield return null;
        }
    }
}

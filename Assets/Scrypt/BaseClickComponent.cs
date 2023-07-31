
using System.Linq;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Checkers
{
    public abstract class BaseClickComponent : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {

        public Coordinate Coordinate;

        //��� �������� �������
        protected MeshRenderer _mesh;

        //��������� �������� �������
        protected Transform _transform;

        //������ ���������� �� ���� �������
        private Material[] _meshMaterials = new Material[3];

        [Tooltip("�������� ������� �������� �������"), SerializeField]
        protected ColorType _color;

        /// <summary>
        /// ���������� �������� ������� �������� �������
        /// </summary>
        public ColorType Color { get => _color; set => _color = value; }

        public Vector3 GetPosition { get => _transform.position; }


        /// <summary>
        /// ���������� ��� ������������� ���� �������� �������
        /// </summary>
        /// <remarks>� ������ ���� - �����, � ����� - ������</remarks>
        public BaseClickComponent Pair { get; set; }

        /// <summary>
        /// ��������� �������������� ��������
        /// </summary>
        public void AddAdditionalMaterial(Material material, int index = 1)
        {
            if (index < 1 || index > 2)
            {
                Debug.LogError("������� �������� ������ ��������. ������ ����� ���� ����� ������ 1 ��� 2");
                return;
            }
            _meshMaterials[index] = material;
            _mesh.materials = _meshMaterials.Where(t => t != null).ToArray();
        }

        /// <summary>
        /// ������� �������������� ��������
        /// </summary>
        public void RemoveAdditionalMaterial(int index = 1)
        {
            if (index < 1 || index > 2)
            {
                Debug.LogError("������� ������� �������������� ��������. ������ ����� ���� ����� ������ 1 ��� 2");
                return;
            }
            _meshMaterials[index] = null;
            _mesh.materials = _meshMaterials.Where(t => t != null).ToArray();
        }

        /// <summary>
        /// ������ �������� �� �����/������
        /// </summary>
        public void ChengeMaterial(Material material)
        {
            AddAdditionalMaterial(material);
            _meshMaterials[2] = _mesh.material;
            _mesh.material = _meshMaterials[1];
        }
        public void ResetMaterial()
        {
            if (_meshMaterials[2] != null) _mesh.material = _meshMaterials[2];
            else _mesh.material = _meshMaterials[0];
            RemoveAdditionalMaterial();
            RemoveAdditionalMaterial(2);
        }

        /// <summary>
        /// ������� ����� �� ������� �������
        /// </summary>
        public event ClickEventHandler OnClickEventHandler;

        /// <summary>
        /// ������� ��������� � ������ ��������� �� ������
        /// </summary>
        public event FocusEventHandler OnFocusEventHandler;


        //��� ��������� �� ������ �����, ���������� ������ �����
        //��� ��������� �� �����, ������ �������������� ������ ��� ���
        //��� ��������� �� ������ - �������������� ���� ������
        public abstract void OnPointerEnter(PointerEventData eventData);

        //���������� ������ OnPointerEnter(), �� ����������� ����� ����� ���������
        //��������� �� ������, �������������� ����� ������� ��������� � ������
        public abstract void OnPointerExit(PointerEventData eventData);

        //��� ������� ������ �� �������, ���������� ������ �����
        public void OnPointerClick(PointerEventData eventData)
        {
            OnClickEventHandler?.Invoke(this);
        }

        //���� ����� ����� ������� � �������� ������� (���� ��� ����) � ��� ����� ���������� �����
        //������� �� ��������� ������ � ������������
        protected void CallBackEvent(CellComponent target, bool isSelect)
        {
            OnFocusEventHandler?.Invoke(target, isSelect);
            
        }

        protected virtual void Start()
        {
            _mesh = GetComponent<MeshRenderer>();
            _transform = GetComponent<Transform>();
            //���� ������ ����� �������������� ��� ������ ���������� � ����,
            //� ������ �� ���������� ������� �� 3 ���������
            //1 ������� - ������ �������� ����, �� �� ��������
            //2 ������� - �������� ��� ��������� ������� �� ������/������ �����
            //3 ������� - �������� ������, �� ������� ����� ����������� �����
            _meshMaterials[0] = _mesh.material;
        }
    }

    public enum ColorType
    {
        White,
        Black
    }

    public delegate void ClickEventHandler(BaseClickComponent component);
    public delegate void FocusEventHandler(CellComponent component, bool isSelect);

    public struct Coordinate
    {
        public int X;
        public int Y;

        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"{X},{Y}";
        }
    }
}
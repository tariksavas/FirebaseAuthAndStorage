namespace Incru.User
{
    using Incru.DataModels;
    using Incru.UI;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;

    public class UserCreation : MonoBehaviour
    {
        [SerializeField] private TMP_InputField usernameInput;

        private UserInformation _userInformation;
        private List<CharacterInformation> _characterInformations = new List<CharacterInformation>();

        private void Awake()
        {
            usernameInput.onValueChanged.AddListener(delegate { RemoveSpaces(); });
        }

        private void OnDestroy()
        {
            usernameInput.onValueChanged.RemoveAllListeners();
        }

        private void RemoveSpaces()
        {
            usernameInput.text = usernameInput.text.Replace(" ", "");
        }

        public void SelectClass(int index)
        {
            _userInformation = new UserInformation
            {
                classType = index,
                userName = usernameInput.text
            };
        }

        public void SelectGender(int index)
        {
            var gender = _characterInformations.Find(x => x.type == 0);

            if (gender != null)
            {
                gender.value = index;
                return;
            }

            _characterInformations.Add(new CharacterInformation
            {
                id = "Gender",
                type = 0,
                value = index
            });
        }

        public void SelectHair(int index)
        {
            var hair = _characterInformations.Find(x => x.type == 1);

            if (hair != null)
            {
                hair.value = index;
                return;
            }

            _characterInformations.Add(new CharacterInformation
            {
                id = "Hair",
                type = 1,
                value = index
            });
        }

        public void SelectSuit(int index)
        {
            var suit = _characterInformations.Find(x => x.type == 2);

            if (suit != null)
            {
                suit.value = index;
                return;
            }

            _characterInformations.Add(new CharacterInformation
            {
                id = "Suit",
                type = 2,
                value = index
            });
        }

        public async void Save()
        {
            //TODO: Handle some errors here!!!

            if (_userInformation == null || string.IsNullOrEmpty(usernameInput.text))
                return;

            LoadingLoop.Instance.StackedLoaders.Add("character-saving");
            await UserManager.Instance.SetUserDataFirstTime(_userInformation, _characterInformations);
            LoadingLoop.Instance.StackedLoaders.Remove("character-saving");
        }
    }
}
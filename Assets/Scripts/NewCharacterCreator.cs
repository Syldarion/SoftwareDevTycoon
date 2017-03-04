using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class NewCharacterCreator : MonoBehaviour
{
    public InputField NameInput;
    public Slider GenderSlider;
    public Dropdown BirthdayDayDropdown;
    public Dropdown BirthdayMonthDropdown;
    public Dropdown BirthdayYearDropdown;
    public Dropdown StartingLocationDropdown;

    public List<SkillAllocationControl> SkillAllocators;

    private readonly string[] birthdayMonths =
    {
        "January", "February", "March", "April",
        "May", "June", "July", "August",
        "September", "October", "November", "December"
    };

    private DateTime birthday;

    void Start()
    {
        PopulateBirthdayDropdowns();
        Location.Locations.Sort(Location.AlphaSortFunction);
        PopulateLocationDropdown();
    }

    void Update()
    {

    }

    private void PopulateBirthdayDropdowns()
    {
        string[] days = new string[31];
        for (int i = 0; i < 31; i++) days[i] = (i + 1).ToString();
        string[] years = new string[DateTime.Today.Year - 18 - 1969];
        for (int i = DateTime.Today.Year - 18; i > 1969; i--) years[DateTime.Today.Year - 18 - i] = i.ToString();

        BirthdayDayDropdown.ClearOptions();
        BirthdayDayDropdown.AddOptions(days.ToList());
        BirthdayMonthDropdown.ClearOptions();
        BirthdayMonthDropdown.AddOptions(birthdayMonths.ToList());
        BirthdayYearDropdown.ClearOptions();
        BirthdayYearDropdown.AddOptions(years.ToList());

        OnBirthdayChange();
    }

    public void OnBirthdayChange()
    {
        int days_in_month = DateTime.DaysInMonth(DateTime.Today.Year - 18 - BirthdayYearDropdown.value,
                                                 BirthdayMonthDropdown.value + 1);

        if (BirthdayDayDropdown.value > days_in_month - 1)
        {
            BirthdayDayDropdown.value = days_in_month - 1;
            BirthdayDayDropdown.RefreshShownValue();
        }

        birthday = new DateTime(
            DateTime.Today.Year - 18 - BirthdayYearDropdown.value,
            BirthdayMonthDropdown.value + 1,
            BirthdayDayDropdown.value + 1);
    }

    private void PopulateLocationDropdown()
    {
        StartingLocationDropdown.ClearOptions();

        List<string> location_names = Location.Locations.Select(loc => loc.Name).ToList();

        StartingLocationDropdown.AddOptions(location_names);
    }

    public void CreateNewCharacter()
    {
        SaveManager.Instance.ActiveSave = null;

        Character new_character = new Character
        {
            Name = NameInput.text,
            Age = DateTime.Today.Month > birthday.Month ||
                  (DateTime.Today.Month == birthday.Month && DateTime.Today.Day > birthday.Day)
                ? DateTime.Today.Year - birthday.Year
                : DateTime.Today.Year - birthday.Year - 1,
            PersonGender = (Person.Gender)GenderSlider.value,
            Birthday = birthday.ToString("dd-MM-yyyy"),
            CurrentLocation = Location.Locations.ElementAt(StartingLocationDropdown.value),
        };

        new_character.AdjustMoney(5000);
        new_character.AdjustReputation(50);

        for (int i = 0; i < SkillInfo.COUNT; i++)
            new_character.Skills[i].Level = SkillAllocators[i].CurrentSkillLevel;

        SceneManager.LoadScene("in_game");

        new_character.SetupEvents();
    }
}

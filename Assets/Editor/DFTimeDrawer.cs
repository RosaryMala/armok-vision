using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DFTime))]
public class DFTimeDrawer : PropertyDrawer
{
    bool unfolded = false;
    int extralines = 7;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var currentYearTicks = property.FindPropertyRelative("currentYearTicks");
        var year = property.FindPropertyRelative("year");
        DFTime time = new DFTime();
        time.CurrentYearTicks = currentYearTicks.intValue;
        time.Year = year.intValue;

        Rect runningPosition = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        unfolded = EditorGUI.Foldout(runningPosition, unfolded, label);

        if (unfolded)
        {
            runningPosition.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(runningPosition, time.ToString(), EditorStyles.boldLabel);

            runningPosition.y += EditorGUIUtility.singleLineHeight;
            int yearLocal = EditorGUI.IntField(runningPosition, "Year", time.Year);
            if (time.Year != yearLocal)
                time.Year = yearLocal;

            runningPosition.y += EditorGUIUtility.singleLineHeight;
            int currentYearTicksLocal = EditorGUI.IntField(runningPosition, "Current Year Ticks", time.CurrentYearTicks);
            if (time.CurrentYearTicks != currentYearTicksLocal)
                time.CurrentYearTicks = currentYearTicksLocal;

            runningPosition.y += EditorGUIUtility.singleLineHeight;
            DFTime.MonthName monthName = (DFTime.MonthName)EditorGUI.EnumPopup(runningPosition, "Month", time.MonthEnum);
            if (time.MonthEnum != monthName)
                time.MonthEnum = monthName;

            runningPosition.y += EditorGUIUtility.singleLineHeight;
            int monthInt = EditorGUI.IntField(runningPosition, "Month", time.Month);
            if (time.Month != monthInt)
                time.Month = monthInt;

            runningPosition.y += EditorGUIUtility.singleLineHeight;
            int day = EditorGUI.IntField(runningPosition, "Day", time.Day);
            if (time.Day != day)
                time.Day = day;

            runningPosition.y += EditorGUIUtility.singleLineHeight;
            int dayTicks = EditorGUI.IntField(runningPosition, "CurrentDayTicks", time.DayTicks);
            if (time.DayTicks != dayTicks)
                time.DayTicks = dayTicks;
        }

        currentYearTicks.intValue = time.CurrentYearTicks;
        year.intValue = time.Year;
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {

        return base.GetPropertyHeight(property, label) + (unfolded ? (EditorGUIUtility.singleLineHeight*extralines) : 0);
    }
}

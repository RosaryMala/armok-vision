using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DFTime))]
public class DFTimeDrawer : PropertyDrawer
{
    bool unfolded = false;
    int extralines = 10;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var ticks = property.FindPropertyRelative("_ticks");
        DFTime time = new DFTime(ticks.longValue);

        Rect runningPosition = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        unfolded = EditorGUI.Foldout(runningPosition, unfolded, label);

        if (unfolded)
        {
            runningPosition.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(runningPosition, time.ToString(), EditorStyles.boldLabel);

            runningPosition.y += EditorGUIUtility.singleLineHeight;
            int yearLocal = EditorGUI.IntField(runningPosition, "Year", time.Year);
            if (time.Year != yearLocal)
                time = new DFTime(yearLocal, time.CurrentYearTicks);

            runningPosition.y += EditorGUIUtility.singleLineHeight;
            int currentYearTicksLocal = EditorGUI.IntField(runningPosition, "Current Year Ticks", time.CurrentYearTicks);
            if (time.CurrentYearTicks != currentYearTicksLocal)
                time = new DFTime(time.Year, currentYearTicksLocal);

            runningPosition.y += EditorGUIUtility.singleLineHeight;
            DFTime.MonthName monthName = (DFTime.MonthName)EditorGUI.EnumPopup(runningPosition, "Month", time.MonthEnum);
            if (time.MonthEnum != monthName)
                time = new DFTime(time.Year, (int)monthName + 1, time.Day, time.Hour, time.Minute, time.Second, time.Millisecond);

            runningPosition.y += EditorGUIUtility.singleLineHeight;
            int monthInt = EditorGUI.IntField(runningPosition, "Month", time.Month);
            monthInt = Mathf.Clamp(monthInt, 1, 12);
            if (time.Month != monthInt)
                time = new DFTime(time.Year, monthInt, time.Day, time.Hour, time.Minute, time.Second, time.Millisecond);

            runningPosition.y += EditorGUIUtility.singleLineHeight;
            int day = EditorGUI.IntField(runningPosition, "Day", time.Day);
            day = Mathf.Clamp(day, 1, 28);
            if (time.Day != day)
                time = new DFTime(time.Year, time.Month, day, time.Hour, time.Minute, time.Second, time.Millisecond);

            runningPosition.y += EditorGUIUtility.singleLineHeight;
            int hour = EditorGUI.IntField(runningPosition, "Hour", time.Hour);
            hour = Mathf.Clamp(hour, 0, 24);
            if (time.Hour != hour)
                time = new DFTime(time.Year, time.Month, time.Day, hour, time.Minute, time.Second, time.Millisecond);

            runningPosition.y += EditorGUIUtility.singleLineHeight;
            int minute = EditorGUI.IntField(runningPosition, "Minute", time.Minute);
            minute = Mathf.Clamp(minute, 0, 59);
            if (time.Minute != minute)
                time = new DFTime(time.Year, time.Month, time.Day, time.Hour, minute, time.Second, time.Millisecond);

            runningPosition.y += EditorGUIUtility.singleLineHeight;
            int second = EditorGUI.IntField(runningPosition, "Second", time.Second);
            second = Mathf.Clamp(second, 0, 59);
            if (time.Second != second)
                time = new DFTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, second, time.Millisecond);

            runningPosition.y += EditorGUIUtility.singleLineHeight;
            int milli = EditorGUI.IntField(runningPosition, "Millisecond", time.Millisecond);
            milli = Mathf.Clamp(milli, 0, 999);
            if (time.Millisecond != milli)
                time = new DFTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second, milli);
        }

        ticks.longValue = time.Ticks;
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {

        return base.GetPropertyHeight(property, label) + (unfolded ? (EditorGUIUtility.singleLineHeight * extralines) : 0);
    }
}

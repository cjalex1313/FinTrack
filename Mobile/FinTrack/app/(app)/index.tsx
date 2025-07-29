import { Text, useColorScheme, View } from "react-native";

export default function HomeScreen() {
  const colorScheme = useColorScheme();
  return (
    <View>
      <Text style={{ color: colorScheme === "dark" ? "white" : "black" }}>
        Home View
      </Text>
    </View>
  );
}

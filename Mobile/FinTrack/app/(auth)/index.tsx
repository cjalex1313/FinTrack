import { useState } from "react";
import { Button, StyleSheet, Text, TextInput, View } from "react-native";

export default function LoginScreen() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  return (
    <View>
      <View style={styles.headerContainer}>
        <Text style={styles.header}>Login page</Text>
      </View>
      <View style={styles.loginFormContainer}>
        <View style={styles.loginInput}>
          <TextInput
            placeholder="Email"
            inputMode="email"
            value={email}
            onChangeText={(text) => setEmail(text)}
          />
        </View>
        <View style={styles.loginInput}>
          <TextInput
            placeholder="Password"
            secureTextEntry={true}
            value={password}
            onChangeText={(text) => setPassword(text)}
          />
        </View>
        <View style={styles.actionsContainer}>
          <View style={styles.actionButton}>
            <Button title="Login" />
          </View>
          <View style={styles.actionButton}>
            <Button title="Register" />
          </View>
        </View>
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  headerContainer: {
    marginVertical: 30,
  },
  header: {
    textAlign: "center",
  },
  loginFormContainer: {
    padding: 20,
  },
  loginInput: {
    marginBottom: 8,
  },
  actionsContainer: {
    flexDirection: "row",
    justifyContent: "space-between",
  },
  actionButton: {
    flex: 1,
    margin: 4,
  },
});

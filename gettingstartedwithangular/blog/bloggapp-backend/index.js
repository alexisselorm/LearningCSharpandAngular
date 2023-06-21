const express = require("express");
const app = express();
const mongoose = require("mongoose");
const bodyParser = require("body-parser");
const url = "mongodb://localhost/blog";
const User = require("./models/user");

mongoose.set("strictQuery", true);
app.use(bodyParser.urlencoded({ extended: true }));
app.use(bodyParser.json());
mongoose.connect(url);

// app.post("/api/user/login", async (req, res) => {
//   console.log(req.body);
//   await mongoose.connect(url);
//   let users = await User.find({ username: "Admin" });
//   console.log(users);
//   return users;
// });

app.post("/api/user/login", async (req, res) => {
  let user = await User.find({
    username: req.body.username,
    password: req.body.password,
  });
  if (user.length > 0) {
    return res.status(200).json({
      status: "success",
      data: user,
    });
  }
  return res.status(200).json({
    status: "fail",
    message: "Login Failed",
  });
});

app.listen(3000, () => console.log("Blog app listening on port 3000!"));
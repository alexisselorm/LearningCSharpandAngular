const mongoose = require("mongoose");
const Schema = mongoose.Schema;

// create a new Schema
const postSchema = new Schema(
  {
    title: { type: "string", required: true },
    text: { type: "string", required: true },
    author_id: { type: Schema.Types.ObjectId, ref: "User" },
  },
  { collection: "post" }
);

const Post = mongoose.model('Post',postSchema)

module.exports = Post;

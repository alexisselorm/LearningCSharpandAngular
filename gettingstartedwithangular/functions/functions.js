"use strict";
function calculateDiscount(price) {
    return price * 0.5;
}
console.log(calculateDiscount(25));
let speed = 101;
let car = {
    speed: 10,
    accelerate: function () {
        this.speed += 10;
        console.log(this.speed);
    },
};
car.accelerate();
let car2 = {
    speed: 10,
    accelerate: function () {
        setTimeout(() => {
            this.speed += 10;
            console.log(this.speed);
        }, 3000);
    },
};
car2.accelerate();

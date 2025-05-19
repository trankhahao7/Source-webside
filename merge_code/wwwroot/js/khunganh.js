const track = document.querySelector('.carousel-track');
const prevButton = document.querySelector('.prev-btn');
const nextButton = document.querySelector('.next-btn');
const scrollAmount = 320; // Khoảng cách mỗi lần trượt

// Nhân đôi nội dung carousel nhiều lần
for (let i = 0; i < 5; i++) { // Nhân 5 lần để đủ dài
    track.innerHTML += track.innerHTML;
}

// Khi click nút Next
nextButton.addEventListener('click', () => {
    track.scrollBy({ left: scrollAmount, behavior: 'smooth' });
});

// Khi click nút Prev
prevButton.addEventListener('click', () => {
    track.scrollBy({ left: -scrollAmount, behavior: 'smooth' });
});

// Auto scroll mỗi 3 giây
setInterval(() => {
    track.scrollBy({ left: scrollAmount, behavior: 'smooth' });
}, 3000);

//    document.addEventListener('DOMContentLoaded', function() {
//    const carousel = document.querySelector('.carousel-container');
//    const prevBtn = document.querySelector('.carousel-arrow.prev');
//    const nextBtn = document.querySelector('.carousel-arrow.next');
//    const dots = document.querySelectorAll('.dot');
//    const cardWidth = document.querySelector('.service-card').offsetWidth;
//    const gap = 25; // Совпадает с gap в CSS
//    let scrollPosition = 0;

//    // Обновление активной точки
//    function updateDots() {
//        const activeIndex = Math.round(scrollPosition / (cardWidth + gap));
//        dots.forEach((dot, index) => {
//        dot.classList.toggle('active', index === activeIndex);
//        });
//    }

//    // Клик по стрелке "Назад"
//    prevBtn.addEventListener('click', () => {
//        scrollPosition = Math.max(scrollPosition - (cardWidth + gap), 0);
//    carousel.scrollTo({
//        left: scrollPosition,
//    behavior: 'smooth'
//        });
//    updateDots();
//    });

//    // Клик по стрелке "Вперед"
//    nextBtn.addEventListener('click', () => {
//        const maxScroll = carousel.scrollWidth - carousel.clientWidth;
//    scrollPosition = Math.min(scrollPosition + (cardWidth + gap), maxScroll);
//    carousel.scrollTo({
//        left: scrollPosition,
//    behavior: 'smooth'
//        });
//    updateDots();
//    });

//    // Клик по точкам
//    dots.forEach((dot, index) => {
//        dot.addEventListener('click', () => {
//            scrollPosition = index * (cardWidth + gap);
//            carousel.scrollTo({
//                left: scrollPosition,
//                behavior: 'smooth'
//            });
//            updateDots();
//        });
//    });

//    // Инициализация при загрузке
//    updateDots();
//});

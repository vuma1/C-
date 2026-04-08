-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Jan 11, 2026 at 09:07 PM
-- Server version: 10.4.32-MariaDB
-- PHP Version: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `coffee_shop_db`
--

-- --------------------------------------------------------

--
-- Table structure for table `feedback`
--

CREATE TABLE `feedback` (
  `id` int(11) NOT NULL,
  `product_rating` varchar(255) DEFAULT NULL,
  `service_rating` varchar(255) DEFAULT NULL,
  `other_comment` text DEFAULT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `star_rating` int(11) DEFAULT 5
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `feedback`
--

INSERT INTO `feedback` (`id`, `product_rating`, `service_rating`, `other_comment`, `created_at`, `star_rating`) VALUES
(1, 'Đồ uống ngon miệng, Trình bày đẹp mắt', 'Nhân viên thân thiện, Không gian sạch sẽ', '', '2025-12-28 15:02:58', 5);

-- --------------------------------------------------------

--
-- Table structure for table `orders`
--

CREATE TABLE `orders` (
  `id` int(11) NOT NULL,
  `total_amount` double NOT NULL,
  `order_date` timestamp NOT NULL DEFAULT current_timestamp(),
  `created_at` timestamp NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `orders`
--

INSERT INTO `orders` (`id`, `total_amount`, `order_date`, `created_at`) VALUES
(1, 80000, '2025-12-22 09:38:17', '2025-12-27 06:16:58'),
(2, 100000, '2025-12-22 09:54:39', '2025-12-27 06:16:58'),
(3, 50000, '2025-12-24 18:04:38', '2025-12-27 06:16:58'),
(4, 75000, '2025-12-24 18:18:46', '2025-12-27 06:16:58'),
(5, 50000, '2025-12-25 16:12:27', '2025-12-27 06:16:58'),
(6, 50000, '2025-12-25 16:13:01', '2025-12-27 06:16:58'),
(7, 60000, '2025-12-25 16:26:35', '2025-12-27 06:16:58'),
(8, 50000, '2025-12-25 16:41:17', '2025-12-27 06:16:58'),
(19, 60000, '2025-12-29 13:59:02', '2025-12-29 13:59:02'),
(20, 55000, '2025-12-29 14:07:51', '2025-12-29 14:07:51'),
(21, 55000, '2025-12-29 14:10:14', '2025-12-29 14:10:14'),
(22, 25000, '2026-01-01 16:34:45', '2026-01-01 16:34:45'),
(23, 25000, '2026-01-01 16:43:11', '2026-01-01 16:43:11'),
(24, 30000, '2026-01-01 17:21:44', '2026-01-01 17:21:44'),
(25, 30000, '2026-01-01 17:23:40', '2026-01-01 17:23:40'),
(26, 30000, '2026-01-01 17:24:46', '2026-01-01 17:24:46'),
(27, 118000, '2026-01-11 15:32:33', '2026-01-11 15:32:33'),
(28, 55000, '2026-01-11 15:42:24', '2026-01-11 15:42:24'),
(29, 25000, '2026-01-11 15:52:07', '2026-01-11 15:52:07');

-- --------------------------------------------------------

--
-- Table structure for table `order_details`
--

CREATE TABLE `order_details` (
  `id` int(11) NOT NULL,
  `order_id` int(11) NOT NULL,
  `product_name` varchar(100) DEFAULT NULL,
  `quantity` int(11) NOT NULL,
  `price` double NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `order_details`
--

INSERT INTO `order_details` (`id`, `order_id`, `product_name`, `quantity`, `price`) VALUES
(1, 3, 'Cà phê sữa đá', 1, 25000),
(2, 3, 'Cacao sữa', 1, 25000),
(3, 4, 'Cà phê đen đá', 3, 25000),
(4, 5, 'Cacao sữa', 2, 25000),
(5, 6, 'Cacao sữa', 2, 25000),
(6, 7, 'Bạc xỉu', 2, 30000),
(7, 8, 'Cacao sữa', 2, 25000);

-- --------------------------------------------------------

--
-- Table structure for table `products`
--

CREATE TABLE `products` (
  `id` int(11) NOT NULL,
  `name` varchar(100) NOT NULL,
  `price` double NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `products`
--

INSERT INTO `products` (`id`, `name`, `price`) VALUES
(1, 'Cà phê đen đá', 25000),
(2, 'Cà phê sữa đá', 25000),
(3, 'Bạc xỉu', 30000),
(4, 'Cacao sữa', 25000),
(6, 'Expresso', 30000),
(7, 'Americano', 35000),
(8, 'Cappuccino', 55000),
(9, 'Latte', 55000),
(10, 'Mocha', 59000),
(11, 'Macchiato', 59000);

-- --------------------------------------------------------

--
-- Table structure for table `tables`
--

CREATE TABLE `tables` (
  `id` int(11) NOT NULL,
  `name` varchar(50) NOT NULL,
  `status` varchar(50) DEFAULT 'Trống'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `tables`
--

INSERT INTO `tables` (`id`, `name`, `status`) VALUES
(1, 'Bàn 01', 'Trống'),
(2, 'Bàn 02', 'Trống'),
(3, 'Bàn 03', 'Trống'),
(5, 'Bàn 04', 'Trống'),
(6, 'Bàn 05', 'Trống'),
(7, 'Bàn 06', 'Trống'),
(8, 'Bàn 07', 'Trống'),
(9, 'Bàn 08', 'Trống'),
(10, 'Bàn 09', 'Trống'),
(12, 'Bàn 10', 'Trống');

-- --------------------------------------------------------

--
-- Table structure for table `users`
--

CREATE TABLE `users` (
  `id` int(11) NOT NULL,
  `username` varchar(50) NOT NULL,
  `password` varchar(100) NOT NULL,
  `role` varchar(20) NOT NULL,
  `salary_rate` double DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `users`
--

INSERT INTO `users` (`id`, `username`, `password`, `role`, `salary_rate`) VALUES
(3, 'Nhanvien1', '123', 'STAFF', 0),
(4, 'Nhanvien2', '123', 'STAFF', 0),
(5, 'Quản lý', '123', 'MANAGER', 0);

--
-- Indexes for dumped tables
--

--
-- Indexes for table `feedback`
--
ALTER TABLE `feedback`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `orders`
--
ALTER TABLE `orders`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `order_details`
--
ALTER TABLE `order_details`
  ADD PRIMARY KEY (`id`),
  ADD KEY `order_id` (`order_id`);

--
-- Indexes for table `products`
--
ALTER TABLE `products`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `tables`
--
ALTER TABLE `tables`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `name` (`name`);

--
-- Indexes for table `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `username` (`username`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `feedback`
--
ALTER TABLE `feedback`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT for table `orders`
--
ALTER TABLE `orders`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=30;

--
-- AUTO_INCREMENT for table `order_details`
--
ALTER TABLE `order_details`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT for table `products`
--
ALTER TABLE `products`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=12;

--
-- AUTO_INCREMENT for table `tables`
--
ALTER TABLE `tables`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=13;

--
-- AUTO_INCREMENT for table `users`
--
ALTER TABLE `users`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `order_details`
--
ALTER TABLE `order_details`
  ADD CONSTRAINT `order_details_ibfk_1` FOREIGN KEY (`order_id`) REFERENCES `orders` (`id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
